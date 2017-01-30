$(function () {
    dmpPoc.init();
    console.log('App initialized.')
})

var dmpPoc = dmpPoc || {
    sendData: function () {
        var url = 'home/sendData/';
        var data = $('#pocForm input[type="number"]').serialize();
        $.post(
            url, 
            data,
            function (data) {
                dmpPoc.showMsg(data);
                dmpPoc.loadDataSets();
            }).done(function () {
                dmpPoc.init();
            }).fail(function () {
                dmpPoc.showMsg('Kommunikationsfejl med backend. Log eventuelt ind igen.');
                })
    },

    init: function () {
        $('#saveDataButton').one('click', function (e) {
            dmpPoc.sendData();
            
        })
    },
    saveDataSet: function(){
    },

    loadDataSets: function(){
        $('#datasetPicker').load('home/DatasetPicker');
    },


    showMsg: function(msg){
        $('#msgBox').val(msg);
    }
};

