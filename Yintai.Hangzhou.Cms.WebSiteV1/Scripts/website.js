$(document).ready(function () {
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