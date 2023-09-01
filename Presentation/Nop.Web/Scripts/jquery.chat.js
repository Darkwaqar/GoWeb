/*
** nopCommerce ajax chat implementation
*/

var Chat = {
    reloadChatUrl: '/PrivateMessages/Conversations?toCustomerId=',
    deleteMessageUrl: '/PrivateMessages/DeleteMessage?privateMessageId=',
    viewMessageUrl: '/PrivateMessages/ViewMessage?privateMessageId=',
    deleteChatUrl: '/PrivateMessages/DeleteAllMessageFromCustomerId?fromCustomerId=',
    readChatUrl: '/PrivateMessages/ReadAllMessageFromCustomerId?fromCustomerId=',
    openChatUrl: '/PrivateMessages/Conversations?toCustomerId=',
    sendChatUrl: '/PrivateMessages/SendPM',
    sendLinkUrl: '/PrivateMessages/SendLink?productId=',
    loadWaiting: false,
    formselector: '',
    conversationselector: '',
    inboxselector: '',
    totalselector: '',
    tocustomerselector: '',
    fromcustomerselector: '',
    messageselector: '',
    ChatHub: '',
    admin: false,

    init: function (formselector, conversationselector, inboxselector, totalselector, tocustomerselector, fromcustomerselector, messageselector, admin) {
        this.loadWaiting = false;
        this.formselector = formselector;
        this.conversationselector = conversationselector;
        this.inboxselector = inboxselector;
        this.totalselector = totalselector;
        this.tocustomerselector = tocustomerselector;
        this.fromcustomerselector = fromcustomerselector;
        this.messageselector = messageselector;
        this.admin = admin;
        this.ChatHub = $.connection.chatHub;
        $.connection.hub.logging = true;
        $.connection.hub.start().done(function () { });
        this.ChatHub.client.addNewMessageToPage = function (ToCustomerId, FromCustomerId) {
            var fromcustomer = $(Chat.fromcustomerselector).val();
            var tocustomer = $(Chat.tocustomerselector).val();
            if (fromcustomer == ToCustomerId) {
                Chat.playSound();
                if (FromCustomerId == tocustomer) {
                    Chat.reloadChat();
                } else {
                    Chat.openChat(FromCustomerId)
                }
            }
        };
        $("#chat-contacts li").first().addClass('active');
        $(Chat.messageselector).focusin(function () { Chat.readChat($(Chat.tocustomerselector).val()) });

    },

    setLoadWaiting: function (display) {
        if (this.admin) {

        }
        else {
            displayAjaxLoading(display);
        }
        this.loadWaiting = display;
    },

    toggleChat: function () {
        $('.direct-chat-contacts').toggle();
        $('.direct-chat-vendors').toggle();
    },

    minimizeChat: function () {
        $('.box-body').toggle();
        $('.box-footer').toggle();
        $('.box-tools').toggle();
        $('.direct-chat').toggleClass("minimize-chat");
        Chat.scrolltolast();
    },

    reloadChat: function () {
        var customerId = $(this.tocustomerselector).val()

        if (customerId == undefined) {
            return;
        }
        if (this.loadWaiting != false) {
            return;
        }
        this.setLoadWaiting(true);
        $.ajax({
            cache: false,
            url: this.reloadChatUrl + customerId,
            type: 'post',
            success: this.success_process,
            complete: this.resetLoadWaiting,
            error: this.ajaxFailure
        })
    },

    deleteMessage: function (messageId) {
        if (this.loadWaiting != false) {
            return;
        }
        this.setLoadWaiting(true);
        $.ajax({
            cache: false,
            url: this.deleteMessageUrl + messageId,
            type: 'post',
            success: this.success_process,
            complete: this.resetLoadWaiting,
            error: this.ajaxFailure
        })
    },
    viewMessage: function (messageId) {
        if (this.loadWaiting != false) {
            return;
        }
        this.setLoadWaiting(true);
        $.ajax({
            cache: false,
            url: this.viewMessageUrl + messageId,
            type: 'post',
            success: this.success_process,
            complete: this.resetLoadWaiting,
            error: this.ajaxFailure
        })
    },

    deleteChat: function (customerId) {
        if (this.loadWaiting != false) {
            return;
        }
        this.setLoadWaiting(true);
        $.ajax({
            cache: false,
            url: this.deleteChatUrl + customerId,
            type: 'post',
            success: this.success_process,
            complete: this.resetLoadWaiting,
            error: this.ajaxFailure
        })
    },
    readChat: function (customerId) {
        if (this.loadWaiting != false) {
            return;
        }
        this.setLoadWaiting(true);
        $.ajax({
            cache: false,
            url: this.readChatUrl + customerId,
            type: 'post',
            success: this.success_process,
            complete: this.resetLoadWaiting,
            error: this.ajaxFailure
        })
    },

    openChat: function (customerId) {
        $("#chat-contacts li").removeClass('active');
        $("#conversations-" + customerId).addClass('active');
        if ($("#conversations-" + customerId).length > 0 && $(".direct-chat-vendors").length > 0 && $(".direct-chat-vendors")[0].style.display != "none") {
            $('#chat-box button[data-toggle="tooltip"]').trigger('click');
        }
        if (this.admin) {
            $('#chat-box button[data-toggle="tooltip"]').trigger('click');
        }

        if (this.loadWaiting != false) {
            return;
        }
        this.setLoadWaiting(true);
        $.ajax({
            cache: false,
            url: this.openChatUrl + customerId,
            type: 'post',
            success: this.success_process,
            complete: this.resetLoadWaiting,
            error: this.ajaxFailure
        })
    },

    sendChat: function () {
        var message = $(this.messageselector).val().trim()

        if (message == undefined || message == "") {
            return;
        }
        if (this.loadWaiting != false) {
            return;
        }
        this.setLoadWaiting(true);
        $.ajax({
            cache: false,
            url: this.sendChatUrl,
            data: $(this.formselector).serialize(),
            type: 'post',
            success: this.success_process,
            complete: this.resetLoadWaiting,
            error: this.ajaxFailure
        })
    },
    sendProductToChat: function (productId) {
        if ($('.direct-chat').hasClass("minimize-chat")) {
            Chat.minimizeChat();
        }
        if (this.loadWaiting != false) {
            return;
        }
        this.setLoadWaiting(true);
        $.ajax({
            cache: false,
            url: this.sendLinkUrl + productId,
            type: 'post',
            success: this.success_process,
            complete: this.resetLoadWaiting,
            error: this.ajaxFailure
        })
    },
    playSound: function () {
        const audio = new Audio("/Themes/DefaultClean/Content/sound/notification.wav");
        audio.play();
    },
    scrolltolast: function () {
        var elem = $(Chat.conversationselector)[0];
        var lay = document.getElementsByClassName('direct-chat-messages')[0];
        if (elem != null) {
            lay.scrollTop = elem.scrollHeight;
        }
    },

    success_process: function (response) {
        if (response.updateconversationsection) {
            $(Chat.conversationselector).replaceWith(response.updateconversationsectionhtml);
        }
        if (response.updateinboxsection) {
            $(Chat.inboxselector).html(response.updateinboxsectionhtml);
        }
        if (response.updateCustomerId) {
            $(Chat.tocustomerselector).val(response.customerId)
            $("#conversations-" + response.customerId).addClass('active');

        }
        if (response.updatetotalsection) {
            $(Chat.totalselector).html(response.totalsectionhtml);
            $("#unread-total-" + $(Chat.tocustomerselector).val()).html(response.totalsectionhtml);
        }
        if (response.updateMessage) {
            $('#chat-' + response.privateMessageId).html(response.updateMessagehtml);

        }
        if (response.scrollToBottom) {
            Chat.scrolltolast();
        }

        if (response.sendcustomerupdate) {
            if (Chat.ChatHub && Chat.ChatHub.state === $.signalR.connectionState.disconnected) {
                Chat.ChatHub.start()
            }
            Chat.ChatHub.server.send($(Chat.tocustomerselector).val(), $(Chat.fromcustomerselector).val())
            $(Chat.messageselector).val("");
        }

        return false;
    },

    resetLoadWaiting: function () {
        Chat.setLoadWaiting(false);
    },

    ajaxFailure: function () {
        alert('Failed to process chat. Please refresh the page and try one more time.');
    }
};