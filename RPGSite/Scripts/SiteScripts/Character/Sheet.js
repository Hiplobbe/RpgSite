$(function() {
    $('#addAttribute').click(function() {
        var dialog = $('<div/>').appendTo('body').dialog({
            resizeable: true,
            height: 500,
            width: 600,
            buttons: {
                Create: function() {
                    var attribute =
                    {
                        Id: 0,
                        Name: $('#Name').val(),
                        MaxValue: $('#MaxValue').val(),
                        Value: $('#Value').val(),
                        AttributeGroupId: $('#ddGroupId').val(),
                        IsStandard: true
                    }

                    $.post({
                        url: "/Character/SaveStandardAttribute",
                        data: attribute,
                        success: function (data) {
                            UpdateAttributeList($.parseJSON(data));
                            dialog.remove();
                        }
                    });
                },
                Cancel: function() {
                    dialog.remove();
                }
            },
            modal: true
        }).load(this.href, {});

        return false;
    });
});

function UpdateAttributeList(data) {
    $('#standardAttributeList').find('optgroup').remove();

    var group;
    $.each(data, function (key, value) {
        if (group == undefined || value.Group.Name != group.attr('label')) {
            group = $('<optgroup/>').attr('label', value.Group.Name);
            $('#standardAttributeList').append(group);
        }
        group.append($('<option/>').attr("value", value.Value).text(value.Text));
    });
}