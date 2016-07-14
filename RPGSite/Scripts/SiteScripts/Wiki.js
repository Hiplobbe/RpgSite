$(document).ready(function () {

    if ($.wikiBack == null) {
        $.current = 'Index';
        $("#wikiBack").hide();
    }

    wikiLinks(false);
});
function toggleButtons() {
    if (!$("#wikiTextArea").is(':visible')) {
        wikiLinks(true);
    }
    else {
        wikiLinks(false);
    }
    $("#wikiHelp").toggle();
    $("#wikiTextDiv").toggle();
    $("#wikiTextArea").toggle();
    $("#wikiEdit").toggle();
    $("#wikiSubmit").toggle();
}
//Makes all the [!] blocks into links in view mode or makes the links into [!] while in edit mode.
function wikiLinks(editMode) {
    if (editMode == false) {
        var regexedit = $("#wikiTextArea").val()
            .replace(/\[!(\w+)\]/g, '<a href="#" onclick="update(\'/Wiki/$1\',\'' + $.current + '\')">$1</a>')
            .replace(/\n/g, '<br/>');
        $("#wikiTextDiv").html(regexedit);
    }
}
function update(link, previous) {
    $.wikiBack = previous;
    $.current = link.match(/\w+$/);

    $.get(link, function (data) {
        $("#wikiTabContent").html(data);
    });
}
function back() {
    update("/Wiki/" + $.wikiBack, $.current);
}