app.controller('APIController', function ($scope, APIService) {
    getAll();

    function getAll() {

        var servCall = APIService.getPIItems();
        servCall.then(function (d) {
            $scope.subscriber = d.data;
        }, function (error) {
            $log.error('Oops! Something went wrong while fetching the data.')
        })
    }

    $scope.saveSubs = function () {

        var sub = {
            mailID: $scope.mailid,
            SubscribedDate: new Date()
        };

        alert($scope.mailid);

        var saveSubs = APIService.saveSubscriber(sub.mailID);
        saveSubs.then(function (d) {
            getAll();

        }, function (error) {

            console.log("Oops! error saving data");
        })

  };



})
