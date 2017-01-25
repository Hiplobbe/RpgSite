//Events
$(function () {
    $('#wikiTab').toggle();

    $.connection.chatHub.client.receiveMessage = recMessage;

    $.connection.hub.start().done(function () {
        $('#sendButton').click(function () {
            sendMessage($('#chatText').val());
            $('#chatText').val('');
        });
    });

    //#region Events
    $("#rollerlist").on('change', function() {
        sendMessage("!roller "+this.value);
    });
    $("#charlist").on('change', function () {
        //TODO:Handle displayname radio button
    });
    $("#chatText").keypress(function (event) {
        var keycode = event.keyCode || event.which;
        if (keycode == '13') {
            sendMessage($('#chatText').val());
            $('#chatText').val('');
        }
    });
    //#endregion
});
//Tab functionality
function tabSelect(sender,tab) {
    $(".jsTabs").hide();
    $(".selectedtab").removeClass("selectedtab");

    $(sender).addClass("selectedtab");
    $(tab).toggle();
}
function updatePartial(link, contentDiv) {
    $(contentDiv).load(link);
}

//Message sender
function sendMessage(message) {
    $.connection.chatHub.server.send(userData.id,message);
}
//Callback
function recMessage(message) {
    //TODO: Handle private messages
    var chatMessage = JSON.parse(message);

    var mClass = getMessageClass(chatMessage.Type);
    
    switch (chatMessage.Type)
    {
        case 2:
            //TODO:Handle success and fail with coloring of message.
            chatMessage.Message = refactorRoll(JSON.parse(chatMessage.Message));
    }

    $(".chatMessages").append("<span class='"+mClass+"'>"+chatMessage.Username + ": " + chatMessage.Message + "</span><br/>");
}
function refactorRoll(Rolls) {
    Rolls.sort(rollSort);
    var message = "";

    for (i = 0; i < Rolls.length; i++) {
        message += Rolls[i].Value + " ";
    }

    return message;
}
function rollSort(x, y) {
    var xValue = x.Value;
    var yValue = y.Value;
    return ((xValue > yValue) ? -1 : ((xValue < yValue) ? 1 : 0));
}
function getMessageClass(type) {
    switch (type) {
        case 0: //Standard
            return "";
        case 1:
            return "whisper";
        case 2:
            return "roll";
    }
}


