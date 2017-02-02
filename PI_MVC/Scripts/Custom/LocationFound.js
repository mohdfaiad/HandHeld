
$(function () {

    $("input[id^='Find_']").on("click", function () {

        var match = $(this).attr('data_match');

        //alert(match);

        var count = 0;

        var row = $(this).attr('data-row');

        $('#content_' + row + ' .found').each(function () {

            var status = $(this).attr("data-status");

            if (status == match)
            {
                $(this).prop('checked', true);
                count++;
            }

        });       

        //alert(count);
    });

});

