﻿$(function () {
    $('#product-list tbody').on('click', '.add-product', function () {
        var productId = $(this).data('id');
        var productName = $(this).data('name');
        var productPrice = $(this).data('price');
        $('#add-product-btn').data('orderid', $(this).data('orderid'));
        var $modal = $('#addProduct');
        var $idField = $('#product-id');
        $idField.val(productId);
        var $nameField = $('#product-name');
        $nameField.val(productName);
        var $priceField = $('#product-price');
        $priceField.val(productPrice);
    });

    $('#add-product-btn').on('click', function () {
        var $tableBody = $('#order-detail tbody');
        var lastIndex = $('#order-detail tbody tr').length;
        if ($('#order-detail tbody tr').find('.dataTables_empty').length > 0)
        {
            $('#order-detail tbody tr').eq(0).remove();
            lastIndex = 0;
        }

        $tableBody.append('<tr></tr>');
        var $currentRow = $('#order-detail tbody tr').eq(lastIndex);
        $currentRow.append('<td><input type="hidden" id="OrderDetails[' + lastIndex.toString() + '].idOrder" name="OrderDetails[' + lastIndex.toString() + '].idOrder" value="' + $(this).data('orderid') + '" class="table-input"/><input type="number" id="OrderDetails[' + lastIndex.toString() + '].idProduct" name="OrderDetails[' + lastIndex.toString() + '].idProduct" value="' + $('#product-id').val() + '" class="table-input"/></td>');
        $currentRow.append('<td>' + $('#product-name').val() + '</td>');
        $currentRow.append('<td><input type="number" id="OrderDetails[' + lastIndex.toString() + '].quantity" name="OrderDetails[' + lastIndex.toString() + '].quantity" value="' + $('#product-quantity').val() + '" class="table-input" /></td>');
        $currentRow.append('<td>' + $('#product-price').val() + '</td>');

        var total = $('#product-price').val() * $('#product-quantity').val();
        $currentRow.append('<td>' + total.toString() + '</td>');

        var totalValue = $('#Total').val();
        if (totalValue == "")
            totalValue = '0';
        var Total = parseInt(totalValue) + parseInt(total);
        $('#Total').val(Total);
    })
})