BloodDonation.AssetStatus= {
    GetAllAssetStatus: ''
};

BloodDonation.AssetStatus.GetAllAssetStatus = function (id, dimmerId) {
    BloodDonation.Datables.ShowDimmer(dimmerId);
    var component = '#' + id;
    $(component).DataTable();

    appClient.get('/assetstatus/getall', null,
        function (response) {
            BloodDonation.AssetStatus.ShowAll(response.data, component, dimmerId);
        })
}

BloodDonation.AssetStatus.ShowAll = function (data, component, dimmerId) {
    $(component).dataTable().fnDestroy();
    $(component).DataTable({
        //"order": [[1, "asc"]],

        "aLengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
        "processing": true,
        "serverSide": false,
        "filter": true,
        "orderMulti": false,
        "bAutoWidth": false,
        "data": data,
        "dom": 'Bfrtip',
        "buttons": [
            'csv', 'excel', 'pdf', 'print'
        ],
        "columnDefs":
            [{
                "targets": [0],
                "visible": false,
                "searchable": false
            }
            ],
        "columns": [

            { "data": "id", "name": "id", "autoWidth": true },
            { "data": "name", "name": "Name", "autoWidth": true },
            { "data": "description", "name": "Description", "autoWidth": true },
            {
                "render": function (data, type, full, meta) {
                    var dt = moment(full.createTime).format('DD-MM-YYYY');
                    var btn = btn = "<span><i class='entypo-calendar'></i>" + dt+" </span>";                   
                    return btn;
                }
            },
            { "data": "createdBy", "name": "Created By", "autoWidth": true },
            {
                "render": function (data, type, full, meta) {
                    var btn = "<a title='Edit' class='label label-info icon-left update' onclick=BloodDonation.AssetStatus.Edit('" + encodeURIComponent(full.id) + "') ><i class='entypo-pencil'></i></a>";
                    btn = btn + "<a title='Delete' class='label label-danger icon-left delete'  onclick=DeleteEntity('" + encodeURIComponent(full.id) + "','Department','" + component + "')> <i class='entypo-trash'></i></a>";
                    return btn;
                }
            }
        ]
    });
    BloodDonation.Datables.HideDimmer(dimmerId);
    BloodDonation.Datables.SetDdl(component);
}

BloodDonation.AssetStatus.Add = function (id) {
    BloodDonation.AssetStatus.ResetForm();
    jQuery.noConflict();

    $('#AssetStatus_crud_modal').modal('show');
}

BloodDonation.AssetStatus.Edit = function (id) {
    $('#entityId').val(id);

    appClient.get('/assetstatus/get/' + id, null,
        function (response) {
            if (response) {
                var data = response.data;
                $('#name').val(data.name);
                $('#description').val(data.description);

                jQuery.noConflict();
                $('#AssetStatus_crud_modal').modal('show');
            }
            else {
                BloodDonation.Settings.Toast('Error', 'An error occured on Getting Asset Status Details', 'error');
            }
        })
}

$("#AssetStatus_crud_frm").submit(function (e) {
    e.preventDefault();
    var id = $('#entityId').val();
    var name = $("#name").val();
    var description = $("#description").val();
    var msg = 'create';
    var api = '';

    if (id === '') {
        appClient.post('/assetstatus/create', {
            name: name,
            description: description
        }, function (response) {
            if (response.data.isSuccess) {
                BloodDonation.Settings.Toast('Success', 'Asset Status  ' + msg + ' has been Succeed', 'Success');
                $('#AssetStatus_crud_modal').modal('hide');
                BloodDonation.Settings.ReloadDt();
            }
            else {
                BloodDonation.Settings.Toast('Error', 'Asset Status ' + msg + ' has been Failed!', 'error');
                $('#AssetStatus_crud_modal').modal('hide');
                BloodDonation.Settings.ReloadDt();
            }
        })

    } else {
        msg = 'update';
        appClient.put('/assetstatus/update/' + id, {
            name: name,
            description: description
        }, function (response) {
            if (response.data.isSuccess) {
                BloodDonation.Settings.Toast('Success', 'Asset Status  ' + msg + ' has been Succeed', 'Success');
                $('#AssetStatus_crud_modal').modal('hide');
                BloodDonation.Settings.ReloadDt();
            }
            else {
                BloodDonation.Settings.Toast('Error', 'Asset Status ' + msg + ' has been Failed!', 'error');
                $('#AssetStatus_crud_modal').modal('hide');
                BloodDonation.Settings.ReloadDt();
            }
        })
    }
});

BloodDonation.AssetStatus.ResetForm = function () {
    $('#entityId').val('');
    $("#name").val('');
    $("#description").val('');
};