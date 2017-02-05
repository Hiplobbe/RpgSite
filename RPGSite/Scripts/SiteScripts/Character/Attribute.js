$(function() {
    $('#newGroupButton').click(function() {
        $('#newGroupButton').hide();
        $('#newGroup').show();
    });
    $('#createNewGroup').click(function() {
        if ($('#newGroupName').val() != "") {
            $.post({
                url: "/Character/SaveAttributeGroup",
                data: { name: $('#newGroupName').val() },
                success: function(data) {
                    if (data != "") {
                        var json = $.parseJSON(data);
                        $('#ddGroupId').append($('<option/>').attr("value", json.Id).text(json.Name));

                        newGroupClose();
                    } else {
                        $('#newGroupError').show();
                    }
                }
            });
        }
    });
});

function newGroupClose() {
    $('#newGroupName').val('');
    $('#newGroup').hide();
    $('#newGroupError').hide();
    $('#newGroupButton').show();
}