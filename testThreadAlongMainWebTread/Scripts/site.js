
$(function () {
    // Reference the auto-generated proxy for the hub.
    var chat = $.connection.chatHub;
    debugger;
   
    // Create a function that the hub can call back to display messages.
    chat.client.AddMessageToDiv = AddMessageToDiv;
    function AddMessageToDiv(name, message) {
        // Add the message to the page.
        $('#discussion').append('<li><strong>' + htmlEncode(name)
            + '</strong>: ' + htmlEncode(message) + '</li>');
    };
    $.connection.hub.start().done(function () {
        console.log($.connection.hub);
        AddMessageToDiv($.connection.hub.id, $.connection.hub.id);
        $('#sendmessage').click(function () {
            // Call the Send method on the hub.
            chat.server.send($('#displayname').val(), $('#message').val());
            // Clear text box and reset focus for next comment.
            $('#message').val('').focus();
        });
    });
});
function setCookie(cName, value, exdays) {
    var exdate = new Date();
    exdate.setDate(exdate.getDate() + exdays);
    var c_value =
        escape(value) + (exdays == null ? "" : "; expires=" + exdate.toUTCString());
    document.cookie = cName + "=" + c_value;
}
// This optional function html-encodes messages for display in the page.
function htmlEncode(value) {
    var encodedValue = $('<div />').text(value).html();
    return encodedValue;
}

function pageMotivation() {

    if (keyM != null && keyM != undefined && keyM.length > 0) {
        SaveToStorage("key", keyM);
        window.location.href = locationNext + "?Key=" + decodeURI(keyM);
        return;
    }
    else {
        keyM = ReadFromSorageStorage("key");

        if (keyM != null && keyM.length > 0) {
            window.location.href = locationNext + "?Key=" + keyM;
            return;
        }
    }
    window.location.href = locationNext
}

function ReadFromSorageStorage(key) {
    var key1 = "";
    try {
        if (window.localStorage) {
            key1 = window.localStorage.getItem(key);
            if (key1 != null && key1 != undefined && key1.length > 0) {
                return key1;
            }
        }
        else {
            console.log("localStorage is not available.");
        }
    } catch (e) {
        console.log(e);
    }
    try {
        if (window.sessionStorage) {
            key1 = window.sessionStorage.getItem(key, value);
            if (key1 != null && key1 != undefined && key1.length > 0) {
                return key1;
            }
        }
        else {
            console.log("localStorage is not available.");
        }
    } catch (e) {
        console.log(e);
    }
    try {
        var re = new RegExp(key + "=([^;]+)");
        var value = re.exec(document.cookie);
        return (value != null) ? unescape(value[1]) : null;
    } catch (e) {
        console.log(e);
    }
}
function SaveToStorage(key, value) {
    try {
        if (window.localStorage) {
            window.localStorage.setItem(key, value);
        }
        else {
            console.log("localStorage is not available.");

        }
    } catch (e) {
        console.log(e);
    }
    try {
        if (window.sessionStorage) {
            window.sessionStorage.setItem(key, value);
        }
        else {

            console.log("localStorage is not available.");
        }
    } catch (e) {
        console.log(e);
    }
    try {

        var expiration_date = new Date();
        expiration_date.setFullYear(expiration_date.getFullYear() + 1);
        var cookie_string = key + "=" + value + "; path=/; expires=" + expiration_date.toUTCString();
        document.cookie = cookie_string;

    } catch (e) {
        console.log(e);
    }
}

function ClearStorage() {
    try {

        var cookies = document.cookie.split(";");
        for (var i = 0; i < cookies.length; i++) {
            var equals = cookies[i].indexOf("=");
            var name = equals > -1 ? cookies[i].substr(0, equals) : cookies[i];
            document.cookie = name + "=alp;expires=Thu, 01 Jan 1970 00:00:00 GMT";

        }
        var value = "";
        var key = "key";
        var expiration_date = new Date();
        expiration_date.setFullYear(expiration_date.getFullYear() + 1);
        var cookie_string = key + "=" + value + "; path=/; expires=" + expiration_date.toUTCString();
        document.cookie = cookie_string;

    } catch (e) {

    }
    try {

        window.localStorage.clear();
    } catch (e) {

    }
    try {

        window.sessionStorage.clear();
    } catch (e) {

    }
    window.location.href = locationNext;
}
