﻿BloodDonation.MaintenanceType = {
    GetAllMaintenanceTypes: ''
};

BloodDonation.MaintenanceType.GetAllMaintenanceTypes = function (id, dimmerId) {
    BloodDonation.Datables.ShowDimmer(dimmerId);
    var component = '#' + id;
    $(component).DataTable();

    appClient.get('/maintenancetypes/getall', null,
        function (response) {
            BloodDonation.MaintenanceType.ShowAll(response.data, component, dimmerId);
        })
}

BloodDonation.MaintenanceType.ShowAll = function (data, component, dimmerId) {
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
                    var dt = moment(full.createTime).format("dddd, MMMM Do YYYY, h:mm:ss a");
                    var btn = btn = "<span><i class='entypo-calendar'></i>" + dt+" </span>";                   
                    return btn;
                }
            },
            { "data": "createdBy", "name": "Created By", "autoWidth": true },
            {
                "render": function (data, type, full, meta) {
                    var btn = "<a title='Edit' class='label label-info icon-left update' onclick=BloodDonation.MaintenanceType.Edit('" + encodeURIComponent(full.id) + "') ><i class='entypo-pencil'></i></a>";
                    btn = btn + "<a title='Delete' class='label label-danger icon-left delete'  onclick=DeleteEntity('" + encodeURIComponent(full.id) + "','Department','" + component + "')> <i class='entypo-trash'></i></a>";
                    return btn;
                }
            }
        ]
    });
    BloodDonation.Datables.HideDimmer(dimmerId);
    BloodDonation.Datables.SetDdl(component);
}

BloodDonation.MaintenanceType.Add = function (id) {
    BloodDonation.MaintenanceType.ResetForm();
    jQuery.noConflict();

    $('#MaintenanceType_crud_modal').modal('show');
}

BloodDonation.MaintenanceType.Edit = function (id) {
    $('#entityId').val(id);

    appClient.get('/maintenancetypes/get/' + id, null,
        function (response) {
            if (response) {
                var data = response.data;
                $('#name').val(data.name);
                $('#description').val(data.description);

                jQuery.noConflict();
                $('#MaintenanceType_crud_modal').modal('show');
            }
            else {
                BloodDonation.Settings.Toast('Error', 'An error occured on Getting Maintenance Type Details', 'error');
            }
        })
}

$("#MaintenanceType_crud_frm").submit(function (e) {
    e.preventDefault();
    var id = $('#entityId').val();
    var name = $("#name").val();
    var description = $("#description").val();
    var msg = 'create';
    var api = '';

    if (id === '') {
        appClient.post('/maintenancetypes/create', {
            name: name,
            description: description
        }, function (response) {
            if (response.data.isSuccess) {
                BloodDonation.Settings.Toast('Success', 'Maintenance Type  ' + msg + ' has been Succeed', 'Success');
                $('#MaintenanceType_crud_modal').modal('hide');
                BloodDonation.Settings.ReloadDt();
            }
            else {
                BloodDonation.Settings.Toast('Error', 'Maintenance Type ' + msg + ' has been Failed!', 'error');
                $('#MaintenanceType_crud_modal').modal('hide');
                BloodDonation.Settings.ReloadDt();
            }
        })

    } else {
        msg = 'update';
        appClient.put('/maintenancetypes/update/' + id, {
            name: name,
            description: description
        }, function (response) {
            if (response.data.isSuccess) {
                BloodDonation.Settings.Toast('Success', 'Maintenance Type  ' + msg + ' has been Succeed', 'Success');
                $('#MaintenanceType_crud_modal').modal('hide');
                BloodDonation.Settings.ReloadDt();
            }
            else {
                BloodDonation.Settings.Toast('Error', 'Maintenance Type ' + msg + ' has been Failed!', 'error');
                $('#MaintenanceType_crud_modal').modal('hide');
                BloodDonation.Settings.ReloadDt();
            }
        })
    }
});

BloodDonation.MaintenanceType.ResetForm = function () {
    $('#entityId').val('');
    $("#name").val('');
    $("#description").val('');
};