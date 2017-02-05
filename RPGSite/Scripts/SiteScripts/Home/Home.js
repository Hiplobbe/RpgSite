$(function() {
    $("#rollerList").change(function() {
        $("#rollerEdit").show();

        var selectedid = $("#rollerlist").val();

        $("#editRoller").attr("href","/Home/EditDiceRoller/?id=" + selectedid);
        $("#deleteRoller").attr("href", "/Home/DeleteDiceRoller/?id=" + selectedid);
    });

    $("#charList").change(function () {
        $("#charEdit").show();

        var selectedid = $("#rollerlist").val();

        $("#editChar").attr("href", "/Character/EditCharacter/?id=" + selectedid);
        $("#deleteChar").attr("href", "/Character/DeleteCharacter/?id=" + selectedid);
    });
});