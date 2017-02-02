$(function () {

    //  alert("Lets roll");


    $(".heading").on("click", function () {

        var index = $(this).attr("data-idx");

        var isVisible = $("#content_" + index).is(':visible');


        if (isVisible) {
            $("#content_" + index).hide();
        }
        else {
            $("#content_" + index).show();
        }




    });

});

