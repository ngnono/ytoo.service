$(document).ready(function () {
    $(".display-label").replaceWith(function () {
        var siblingField = $(this).siblings(".display-field:first");
        var siblingText = siblingField.html();
        siblingField.remove();
        return "<tr><td>" + $(this).html() + "</td><td>" + siblingText + "</td></tr>";
    });
    $("fieldset").replaceWith(function () {
        return "<table>" + $(this).html() + "</table>";
    });
    $("table").addClass("table table-bordered table-condensed table-striped");

    $(".datetimepicker").datetimepicker({
        timeText: '时间',
        hourText: '小时',
        minuteText: '分钟',
        secondText: '秒',
        currentText: '现在',
        closeText: '确定'
    });
});