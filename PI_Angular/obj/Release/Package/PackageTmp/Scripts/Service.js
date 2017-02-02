app.service("APIService", function ($http) {
    this.getPIItems = function () {

        return $http.get("http://localhost/PI_Api/api/physicalinventory/Items")
    }


    this.saveSubscriber = function (sub) {

        alert(sub);
        return $http(
       {
           method: 'post',
           data: sub,
           url: 'http://localhost/PI_Api/api/Physicalinventory/AddItem'
       });

       alert("Hello")
       
    }




});
