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
                $('#pocForm input[type="number"]').val('');
                dmpPoc.init();
            }).fail(function () {
                dmpPoc.showMsg('Kommunikationsfejl med backend. Log eventuelt ind igen.');
                })
    },

    init: function () {
        $('#saveDataButton').off();
        $('#saveDataButton').on('click', function (e) {
            dmpPoc.sendData();
        });
        $('#calculateButton').off();
        $('#calculateButton').on('click', function (e) {
            dmpPoc.getCalculatedDataSet();
        });
        
    },
    getCalculatedDataSet: function () {
        var id = $('#datasetPicker option:selected').val().substr(3);
        var url = '/home/calculateDataset?id=' + id;
        $.post(url, function (data) {
            var msg = data.Message + '\nA= ' + data.ValueA + '\nB = ' + data.ValueB + '\nC = ' + data.ValueC;
            dmpPoc.showMsg(msg);
        }).done(function () {
            dmpPoc.init();
        }).fail(function () {
            dmpPoc.showMsg('You failed getting data.');
        });
    },

    loadDataSets: function(){
        $('#datasetPicker').load('home/DatasetPicker');
    },


    showMsg: function(msg){
        $('#msgBox').val(msg);
    }
};

