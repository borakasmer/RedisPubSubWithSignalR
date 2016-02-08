var app = angular.module("app", ["directApp", "itemApp"]);
app.controller('ProductController', function ($scope, $http) {

    var hubproxy = $.connection.rank;
    $.connection.hub.logging = true;
    $.connection.hub.start(function () {
        console.log("hub.start.done");
    });
    hubproxy.client.getRank = function (rankList) {
        $scope.Ranks = rankList;
        $scope.$apply()
        console.log("client.getRank.done");
        console.log(JSON.stringify(rankList));
    }
    //-------------------------------------------------

    $scope.Products = [];
    $http({
        method: 'GET',
        url: '/Home/GetProducts'
    }).success(function (result) {
        console.log(JSON.stringify(result));
        $scope.Products = result;
        //console.log($scope.selectedProduct);
    });

    $http({
        method: 'GET',
        url: '/Home/GetRanks'
    }).success(function (result) {
        console.log("RankList: "+JSON.stringify(result));
        $scope.Ranks = result;
    });

    $scope.GetItems = function () {
        var productID = $scope.selectedProduct;
        console.log(productID);
        $http({
            method: 'POST',
            url: '/Home/GetItems',
            data: { 'productID': productID }
        }).success(function (result) {
            console.log(JSON.stringify(result));
            $scope.Items = result;
        });
    }
    $scope.EditItem = function (ProductID, ItemID) {
        //console.log("EditTem ProductID: " + ProductID+","+Name+","+Price);       
        //console.log(JSON.stringify($scope.EditedItem));
        document.location.href = "/EditItem/" + ProductID + "/" + ItemID;
    }
});

var directApp = angular.module("directApp", []);
directApp.controller('SubController', function ($scope, $http, $location) {
    $scope.SaveProduct = function () {
        var product = {
            "Name": $scope.Name,
            "Detail": $scope.Detail
        };
        $http({
            method: 'POST',
            url: '/Home/SaveProduct',
            data: product
        }).success(function () {
            document.location.href = "/";
        });
    }
});

var itemApp = angular.module("itemApp", []);
directApp.controller('ItemController', function ($scope, $http) {
    $scope.SaveItem = function () {
        var item = {
            "Name": $scope.Name,
            "Price": $scope.Price,
            "ProductID": document.getElementById("hdnProductID").value
        };
        $http({
            method: 'POST',
            url: '/Home/SaveItem',
            data: item
        }).success(function () {
            document.location.href = "/";
        });
    }
});