// 解決Uncaught TypeError
jQuery.fn.pickadate = jQuery.fn.pickadate || {};
jQuery.fn.pickatime = jQuery.fn.pickatime || {};

// Traditional Chinese
jQuery.extend( jQuery.fn.pickadate.defaults, {
    monthsFull: [ '1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月' ],
    monthsShort: [ '1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月'],
    weekdaysFull: [ '週日', '週一', '週二', '週三', '週四', '週五', '週六' ],
    weekdaysShort: [ '日', '一', '二', '三', '四', '五', '六' ],
    today: '今天',
    clear: '清除',
    close: '關閉',
    firstDay: 0,
    format: 'yyyy 年 mm 月 dd 日',
    formatSubmit: 'yyyy/mm/dd'
});

jQuery.extend( jQuery.fn.pickatime.defaults, {
    clear: '關閉'
});
