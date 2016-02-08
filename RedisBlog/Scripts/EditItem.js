var app = angular.module("app", []);
app.controller('EditItemController', function ($scope, $http) {    
    $http({
        method: 'POST',
        url: '/Home/GetEditItem',        
        data: { 'ProductID': document.getElementById("hdnProductID").value, 'Id': document.getElementById("hdnItemID").value }
    }).success(function (result) {
        console.log("Edit Item: "+JSON.stringify(result));
        $scope.EditedItem = result;        
    });

    $scope.UpdateItem = function () {
        var item = {
            "Name": $scope.EditedItem.Name,
            "Price": $scope.EditedItem.Price,
            "ProductID": document.getElementById("hdnProductID").value,
            "Id": document.getElementById("hdnItemID").value,
        };
        $http({
            method: 'POST',
            url: '/Home/UpdateItem',
            data: item
        }).success(function () {
            document.location.href = "/";
        });
    }
});