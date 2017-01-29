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
                console.log('færdig');
            })
    },

    init: function () {
        $('#saveDataButton').on('click', function (e) {
            e.preventDefault();
            e.stopPropagation();
            dmpPoc.sendData();

        })
    },
    loadDataSets(){
        $('#datasetPicker').load('home/DatasetPicker');
    },


    showMsg: function(msg){
        $('#msgBox').val(msg);
    }
};

