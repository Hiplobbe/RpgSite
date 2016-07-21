//Events
$(function () {
    tabSelect('#wikiTab');

    $.connection.chatHub.recFunc = recMessage;

    $.connection.hub.start().done(function () {
        $('#sendButton').click(function () {
            sendMessage(userData.id,$('#chatText').val()); //TODO: Authorize chat page.
        });
    });
});

//Tab functionality
function tabSelect(tab) {
    $(".jsTabs").hide();
    $(tab).toggle();
}
function updatePartial(link, contentDiv) {
    $(contentDiv).load(link);
}

//Message sender
function sendMessage(id,message) {
    $.connection.chatHub.server.send(id,message);
}
//Callback
function recMessage(sender, message) {
    //TODO: Finalise.
}