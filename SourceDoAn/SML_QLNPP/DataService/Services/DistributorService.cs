﻿using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataModel;
using DataService.Interfaces;
using DataModel.Interfaces;
using DataModel.Repositories;
using DataService.Dtos;
using NLog;

namespace DataService.Services
{
    public class DistributorService : IDistributorService
    {
        ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Distributor> _distributorRepository;
        private readonly IRepository<Contract> _contractRepository;
        IAccountService _serviceAccount;
    
        /// <summary>
        /// Hàm khởi tạo
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <returns></returns>
        public DistributorService(IUnitOfWork unitOfWork, IAccountService accountService)
        {
            _unitOfWork = unitOfWork;
            _distributorRepository = unitOfWork.Repository<Distributor>();
            _contractRepository = unitOfWork.Repository<Contract>();
            _serviceAccount = accountService;
        }

        public bool hasContract(int distributorId)
        {
            var allContracts = _contractRepository.GetAll(x => x.distributor == distributorId);
            if (allContracts.Count() == 0 || allContracts.All(c => c.expiredDate < DateTime.Now))
                return false;
            return true;
        }

        public bool priceOverDebt(int distributorId, decimal price)
        {
            if (_contractRepository.Get(x => x.distributor == distributorId && x.beginDate <= DateTime.Now && x.expiredDate > DateTime.Now).maxDebt < price)
                return true;
            return false;
        }

        public bool CheckEmail(string email)
        {
            throw new NotImplementedException();
        }

        public bool CheckPhone(string phone)
        {
            throw new NotImplementedException();
        }

        public int GenerateDistributorId()
        {
            var latestDis = _distributorRepository.GetAll().OrderByDescending(x => x.idDistributor).FirstOrDefault();
            if (latestDis != null)
                return latestDis.idDistributor + 1;
            else
                return 0;
        }

        public int Create(Distributor person)
        {
            logger.Info("Start to create a distributor...");
            person.idDistributor = GenerateDistributorId();
            person.createdDate = DateTime.Now;
            person.updatedDate = DateTime.Now;
            person.debt = 0;
            person.status = true;
            try
            {
                _distributorRepository.Add(person);
                
            }
            catch (Exception ex)
            {
                logger.Info(ex.Message);
                throw ex;
            }
            return person.idDistributor;
        }

        public IList<DistributorList> GetList(Nullable<int> id = null, bool? status = null)
        {
            logger.Info("Start service....");
            IList<Distributor> ds_Dis = new List<Distributor>();
            if (status == false)
            {
                ds_Dis = _distributorRepository.GetAll(x => x.status == status).ToList();
            }
            else
            if (id == null)
                ds_Dis = _distributorRepository.GetAll().ToList();
            else
                ds_Dis = _distributorRepository.GetAll(x => x.idDistributor == id).ToList();
            IList<DistributorList> listDis = new List<DistributorList>();
            DistributorList lDis;
            foreach (Distributor dis in ds_Dis)
            {
                lDis = new DistributorList();
                lDis.Dis = dis;
                foreach (Contract con in dis.Contracts)
                    if (con.status == true)
                    {
                        lDis.Area = con.area;
                        lDis.Dis_Type = con.disType;
                        break;
                    }
                listDis.Add(lDis);
            }
            logger.Info("End service...");
            return listDis;
        }

        public Distributor SearchByID(int id )
        {
            logger.Info("Start service....");
            Distributor dis = _distributorRepository.Get(x => x.idDistributor == id);
            logger.Info("End service...");
            return dis;
        }

        public bool UpdateDebt(int id, long money)
        {
            //throw new NotImplementedException();
            logger.Info("Start service....");
            try {
                var distributor = this.SearchByID(id);
                distributor.debt = distributor.debt + money;
                _unitOfWork.SaveChange();
                logger.Info("End service...");
                return true;
            }
            catch (Exception e)
            {
                logger.Info(e.Message);
                logger.Info("End service...");
                return false;
            }
        }

        public bool UpdateStatus(int id, bool status, string note)
        {
            logger.Info("Start to update status of the distributor...");
            try
            {
                Distributor dis = SearchByID((int)id);
                dis.status = status;
                dis.updatedDate = DateTime.Now;
                dis.note = note;

                _distributorRepository.Update(dis);
                _unitOfWork.SaveChange();

                // Update status of account's Distributor
                _serviceAccount.UpdateStatus(dis.name, note, !status);
                
                logger.Info("End: Successful...");
                return true;
            }
            catch(Exception ex)
            {
                logger.Info(ex.Message);
                return false;
            }
        }

        public IList<DistributorList> GetAll()
        {
            throw new NotImplementedException();
        }


        public List<Distributor> SearchByName(string searchTerm)
        {
            var distributors = _distributorRepository.GetAll(x => x.name.Contains(searchTerm) && x.status == true).ToList();
            return distributors;
        }

    }
}
