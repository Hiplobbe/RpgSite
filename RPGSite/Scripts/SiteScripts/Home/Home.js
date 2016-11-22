$(function() {
    $("#rollerlist").change(function() {
        $("#rolleredit").show();

        var selectedid = $("#rollerlist").val();

        $("#editRoller").attr("href","/Home/EditDiceRoller/?id=" + selectedid);
        $("#deleteRoller").attr("href", "/Home/DeleteDiceRoller/?id=" + selectedid);
    });
});

function EditRoller() {
    $.post
}