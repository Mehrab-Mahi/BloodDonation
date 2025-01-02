var BloodDonation = {};
BloodDonation.Settings = {};
BloodDonation.Datables = {};
BloodDonation.User = {};
BloodDonation.Settings.Toast = function (heading, text, icon) {
    $.toast({
        heading: heading,
        text: text,
        showHideTransition: 'slide',
        icon: icon
    })
}

BloodDonation.Settings.ConvertToBase64 = function (btn, elementId) {
    var id = '#' + elementId;
    if (btn.files && btn.files[0]) {
        var f = btn.files[0];

        var reader = new FileReader();
        reader.onload = (function (theFile) {
            return function (e) {
                var binaryData = e.target.result;
                $(id).val(binaryData);
            };
        })(f);

        reader.readAsDataURL(btn.files[0]);
    }
}

//Common
BloodDonation.Datables.ShowDimmer = function (dimmerId) {
    if (dimmerId !== '') {
        var dimmer = '#' + dimmerId;
        $(dimmer).show();
    }
}

BloodDonation.Datables.HideDimmer = function (dimmerId) {
    if (dimmerId !== '') {
        var dimmer = '#' + dimmerId;
        $(dimmer).hide();
    }
}

BloodDonation.Datables.SetDdl = function (component) {
    $(component).closest('.dataTables_wrapper').find('select').select2({
        minimumResultsForSearch: -1
    });
}

BloodDonation.Settings.ReloadDt = function () {
    setTimeout(function () {
        location.reload();
    }, 3000);
}

BloodDonation.Settings.DeleteConfirm = function () {
    var id = $('#modal_entity_id').val();
    var table = $('#modal_sql_table_id').val();
    var component = $('#modal_component_id').val();

    $.ajax({
        url: "/settings/deleteentity",
        type: "POST",
        data: {
            id: id,
            table: table
        },
        dataType: "json",
        success: function (response) {
            if (response.status) {
                BloodDonation.Settings.Toast('Success', 'Entity has been deleted Successfully', 'Success');
                jQuery.noConflict();
            } else {
                BloodDonation.Settings.Toast('Error', response.message, 'error');
                jQuery.noConflict();
            }

            $('#confirm_delete').modal('hide');
            BloodDonation.Settings.ReloadDt();
        },
        error: function (e) {
            alert(e.statusText);
        }
    });
}

function DeleteEntity(entityId, dbTable, componentName) {
    var id = decodeURIComponent(entityId);
    var table = decodeURIComponent(dbTable);
    var component = decodeURIComponent(componentName);

    $('#modal_entity_id').val(id);
    $('#modal_sql_table_id').val(table);
    $('#modal_component_id').val(component);

    jQuery.noConflict();
    $('#confirm_delete').modal('show');
}

function EditEntity(entityId, dbTable, componentName) {
    var id = decodeURIComponent(entityId);
    var table = decodeURIComponent(dbTable);
    var component = decodeURIComponent(componentName);

    if (table === 'User') {
        BloodDonation.User.Edit(id);
    }
}

BloodDonation.Settings.SetMenuActive = function (menu, action) {
    var roleId = $('#currentUserRole').val();
    var keyName = 'role_' + roleId;
    var menuList = window.localStorage.getItem(keyName);
    var data = JSON.parse(menuList);
    var menuItem = _.find(data, function (item) {
        return item.id === menu;
    });
    if (menuItem) {
        for (var i = 0; i < menuItem.child.length; i++) {
            var childMenuId = '#' + menuItem.child[i].id;
            $(childMenuId).addClass("visible");
        }
    }

    var menuId = '#' + menu;
    var actionId = '#' + action;

    BloodDonation.Settings.DeactiveAllMenu();
    $(menuId).addClass("active opened parent_menu");
    $(actionId).addClass("active");
}

BloodDonation.Settings.DeactiveAllMenu = function () {
}

BloodDonation.User.GetCurentUser = function () {
    appClient.get('/auth/currentuser', null,
        function (response) {
            window.localStorage.setItem('currentuser', response);
        })
}

//User Management
BloodDonation.Datables.GetAllUser = function (id, dimmerId) {
    BloodDonation.Datables.ShowDimmer(dimmerId);
    var component = '#' + id;
    $(component).DataTable();

    appClient.get('/users/getall', null,
        function (response) {
            BloodDonation.Datables.ShowAllUser(response.data, component, dimmerId);
        })
}

BloodDonation.Datables.ShowAllUser = function (data, component, dimmerId) {
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
            { "data": "id", "name": "User Id", "autoWidth": true },
            { "data": "firstName", "name": "First Name", "autoWidth": true },
            { "data": "lastName", "name": "Last Name", "autoWidth": true },
            { "data": "emailAddress", "name": "Email", "autoWidth": true },
            { "data": "userName", "name": "User Name", "width": "8%" },
            {
                "render": function (data, type, full, meta) {
                    var btn = "";
                    if (full.isSuperAdmin) {
                        btn = "<span class='label label-primary'><i class='entypo-lock'></i>Super Admin </span>";
                    }
                    else {
                        if (full.roleName === 'No Role') {
                            btn = "<span class='label label-danger'><i class='entypo-lock'></i>No Role</span>";
                        } else {
                            btn = "<span class='label label-success'>" + full.roleName + "</span>";
                        }
                    }

                    return btn;
                }, "width": "10%"
            },
            {
                "render": function (data, type, full, meta) {
                    var dt = moment(full.createTime).format('DD-MM-YYYY');
                    var btn = btn = "<span><i class='entypo-calendar'></i>" + dt + " </span>";
                    return btn;
                }, "width": "20%"
            },
            { "data": "createdBy", "name": "Created By", "width": "8%" },
            {
                "render": function (data, type, full, meta) {
                    var btn = "<a title='Reset Password' class='label label-reset icon-left update' onclick=BloodDonation.User.ResetPassword('" + encodeURIComponent(full.id) + "') ><i class='entypo-eye'></i></a>";
                    btn = btn + "<a title='Edit' class='label label-info icon-left update' onclick=EditEntity('" + encodeURIComponent(full.id) + "','User','" + component + "') ><i class='entypo-pencil'></i></a>";
                    btn = btn + "<a title='Delete' class='label label-danger icon-left delete'  onclick=DeleteEntity('" + encodeURIComponent(full.id) + "','User','" + component + "')> <i class='entypo-trash'></i></a>";

                    return btn;
                }
            },
        ]
    });
    BloodDonation.Datables.HideDimmer(dimmerId);
    BloodDonation.Datables.SetDdl(component);
}

BloodDonation.User.ResetCrudForm = function () {
    $("#firstName").val('');
    $("#lastName").val('');
    $('#email').val('');
    $('#userName').val('');
    $('#organization').val('');
    $('#phone').val('');
    $('#position').val('');
    $('#entityId').val('0');
    $('#roleid').empty();
}

BloodDonation.User.ResetPassForm = function () {
    $("#oldPass").val('');
    $("#newPass").val('');
    $("#confirmPass").val('');

    $('#modal_entity_id').val('0');
}

BloodDonation.User.ShowPass = function () {
    var oldPassProp = $('#oldPass');
    var newPassProp = $('#newPass');
    var verifyPassProp = $('#confirmPass');

    var oldPassType = oldPassProp.prop('type') == 'password' ? 'text' : 'password';
    oldPassProp.prop('type', oldPassType);

    var newPassType = newPassProp.prop('type') == 'password' ? 'text' : 'password';
    newPassProp.prop('type', newPassType);

    var verifyPassType = verifyPassProp.prop('type') == 'password' ? 'text' : 'password';
    verifyPassProp.prop('type', verifyPassType);
}

BloodDonation.User.ResetPassword = function (entityId) {
    BloodDonation.User.ResetPassForm();
    var id = decodeURIComponent(entityId);

    jQuery.noConflict();
    $('#modal_entity_id').val(id);

    $('#User_reset_pass').modal('show');
}

BloodDonation.User.Add = function () {
    BloodDonation.User.ResetCrudForm();
    jQuery.noConflict();
    $('#passInput').show();

    appClient.get('/roles/getall', null,
        function (response) {
            for (var i = 0; i < response.data.length; i++) {
                $('#roleid').append('<option value=' + response.data[i].id + '> ' + response.data[i].name + ' </option>');
            }
            $('#User_crud_modal').modal('show');
        })
}

BloodDonation.User.Edit = function (id) {
    var header = '#myModalLabel';
    $(header).text('Edit User');
    BloodDonation.User.ResetCrudForm();

    appClient.get('/roles/getall', null,
        function (response) {
            for (var i = 0; i < response.data.length; i++) {
                $('#roleid').append('<option value=' + response.data[i].id + '> ' + response.data[i].name + ' </option>');
            }
        })
    appClient.get('/users/GetById/' + id, null,
        function (response) {
            if (response.data) {
                var model = response.data;
                $('#firstName').val(model.firstName);
                $('#middleName').val(model.middleName);
                $('#lastName').val(model.lastName);
                $('#email').val(model.emailAddress);
                $('#userName').val(model.userName);
                $('#entityId').val(model.id);
                $('#passInput').hide();
                $('#roleid').val(model.roleId);

                jQuery.noConflict();
                $('#User_crud_modal').modal('show');
            }
            else {
                BloodDonation.Settings.Toast('Error', 'An error occured on Getting User Details', 'error');
            }
        })
}

$("#User_crud_frm").submit(function (e) {
    e.preventDefault();

    var isVendor = true;
    var firstName = $('#firstName').val();
    var lastName = $('#lastName').val();
    var email = $('#email').val();
    var userName = $('#userName').val();
    var id = $('#entityId').val();
    var password = $('#newPassword').val();
    var roleid = $('#roleid').val();

    if (id == "0") {
        msg = "Update";
    }

    if (firstName && lastName && email) {
        if (id === "0") {
            appClient.post('/users/create', {
                firstName: firstName,
                lastName: lastName,
                emailAddress: email,
                userName: userName,
                password: password,
                roleid: roleid,
                isApproved: true
            },
                function (response) {
                    if (response.data.isSuccess) {
                        BloodDonation.Settings.Toast('Success', 'User  Creation has been Succeed', 'Success');
                        BloodDonation.User.ResetCrudForm();

                        jQuery.noConflict();
                        $('#User_crud_modal').modal('hide');
                        BloodDonation.Settings.ReloadDt();
                    }
                    else {
                        BloodDonation.Settings.Toast('Error', response.data.message, 'error');
                        BloodDonation.User.ResetCrudForm();

                        jQuery.noConflict();
                        $('#User_crud_modal').modal('hide');
                        BloodDonation.Settings.ReloadDt();
                    }
                })
        }
        else {
            appClient.put('/users/update/' + id, {
                firstName: firstName,
                lastName: lastName,
                emailAddress: email,
                roleid: roleid
            },
                function (response) {
                    if (response.data.isSuccess) {
                        BloodDonation.Settings.Toast('Success', 'User  Update has been Succeed', 'Success');
                        BloodDonation.User.ResetCrudForm();

                        jQuery.noConflict();
                        $('#User_crud_modal').modal('hide');
                        BloodDonation.Settings.ReloadDt();
                    }
                    else {
                        BloodDonation.Settings.Toast('Error', 'User Update became unsuccessful', 'error');
                        BloodDonation.User.ResetCrudForm();

                        jQuery.noConflict();
                        $('#User_crud_modal').modal('hide');
                        BloodDonation.Settings.ReloadDt();
                    }
                })
        }
    }
    else {
        return;
    }
});

$("#User_pass_frm").submit(function (e) {
    e.preventDefault();
    var newPassword = $('#newPass').val();
    var oldPassword = $('#oldPass').val();
    var id = $('#modal_entity_id').val();

    if (newPassword) {
        appClient.post('/users/resetpassword', {
            oldPassword: oldPassword,
            newPassword: newPassword,
            id: id
        },
            function (response) {
                if (response.data.isSuccess) {
                    BloodDonation.Settings.Toast('Success', 'User Password reset has been Succeed', 'Success');
                    BloodDonation.User.ResetPassForm();

                    jQuery.noConflict();
                    $('#User_reset_pass').modal('hide');
                }
                else {
                    BloodDonation.Settings.Toast('Error', 'User  Password reset became unsuccessful', 'error');
                    BloodDonation.User.ResetPassForm();

                    jQuery.noConflict();
                    $('#User_reset_pass').modal('hide');
                }
            })
    }
    else {
        return;
    }
});