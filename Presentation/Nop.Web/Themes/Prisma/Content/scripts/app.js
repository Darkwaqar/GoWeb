// Initialize Firebase
var config = {
    apiKey: "AIzaSyDcUaKH22dOCAU6ZBFx35RRmAb4l6-vZc4",
    authDomain: "mobishop-let-s-shop-instyle.firebaseapp.com",
    databaseURL: "https://mobishop-let-s-shop-instyle.firebaseio.com",
    projectId: "mobishop-let-s-shop-instyle",
    storageBucket: "mobishop-let-s-shop-instyle.appspot.com",
    messagingSenderId: "363361383809"
};

firebase.initializeApp(config);

const messaging = firebase.messaging();

messaging.getToken().then(function (currentToken) {
    if (currentToken) {
        sendTokenToServer(currentToken);
    } else {
        // Show permission UI.
        setTokenSentToServer(false);
        messaging.requestPermission()
            .then(function () {
                console.log('have permission');
                return messaging.getToken();
            })
            .then(function (token) {
                sendTokenToServer(token);
            })
            .catch(function (err) {
                console.log('Error Occured');
            })
    }
}).catch(function (err) {
    setTokenSentToServer(false);
    messaging.requestPermission()
        .then(function () {
            console.log('have permission');
            return messaging.getToken();
        })
        .then(function (token) {
            sendTokenToServer(token);
        })
        .catch(function (err) {
            console.log('permission denied');
        })
});


// [START refresh_token]
// Callback fired if Instance ID token is updated.
messaging.onTokenRefresh(function () {
    messaging.getToken().then(function (refreshedToken) {
        console.log('Token refreshed.');
        // Indicate that the new Instance ID token has not yet been sent to the
        // app server.
        setTokenSentToServer(false);
        // Send Instance ID token to app server.
        sendTokenToServer(refreshedToken);
    }).catch(function (err) {
        console.log('Unable to retrieve refreshed token ', err);
    });
});
// [END refresh_token]


function sendTokenToServer(currentToken) {
    if (!isTokenSentToServer()) {
        console.log('Sending token to server...');
        var e = module.init()
        var postData1 = {
            DeviceId: currentToken,
            Package: window.location.hostname,
            DeviceOS: 3,
            Brand: e.os.name,
            OSVersion: e.os.version,
            Carrier: e.browser.name
        };
        $.ajax({
            cache: false,
            type: "Post",
            url: window.location.origin + "/Mservices/customer/AddDevice",
            data: postData1,
            success: function (data, textStatus, xhr) {
                if (data.status) {
                    console.log(data);
                    // TODO(developer): Send the current token to your server.
                    setTokenSentToServer(true);
                }
                else {
                    setTokenSentToServer(false);
                }
                
            },
            error: function (xhr, textStatus, errorThrown) {
                console.log(errorThrown);
            }
        });
       
    } else {
        console.log('Token already sent to server so won\'t send it again ' +
            'unless it changes');
    }
}

function isTokenSentToServer() {
    return window.localStorage.getItem('sentToServer') === '1';
}
function setTokenSentToServer(sent) {
    window.localStorage.setItem('sentToServer', sent ? '1' : '0');
}


messaging.onMessage(function (payload) {
    console.log('OnMessage: ', payload);
    const notificationTitle = payload.notification.title;
    const notificationOptions = {
        body: payload.notification.body,
        icon: payload.notification.icon,
        image: payload.data.image,
        badge: payload.data["gcm.notification.badge"],
    };

    if (!("Notification" in window)) {
        console.log("This browser does not support system notifications");
    }
    // Let's check whether notification permissions have already been granted
    else if (Notification.permission === "granted") {
        // If it's okay let's create a notification
        var notification = new Notification(notificationTitle, notificationOptions);
        notification.onclick = function (event) {
            event.preventDefault(); // prevent the browser from focusing the Notification's tab
            window.open(payload.notification.click_action, '_blank');
            notification.close();
        }
    }
});


var module = {
    options: [],
    header: [navigator.platform, navigator.userAgent, navigator.appVersion, navigator.vendor, window.opera],
    dataos: [
        { name: 'Windows Phone', value: 'Windows Phone', version: 'OS' },
        { name: 'Windows', value: 'Win', version: 'NT' },
        { name: 'iPhone', value: 'iPhone', version: 'OS' },
        { name: 'iPad', value: 'iPad', version: 'OS' },
        { name: 'Kindle', value: 'Silk', version: 'Silk' },
        { name: 'Android', value: 'Android', version: 'Android' },
        { name: 'PlayBook', value: 'PlayBook', version: 'OS' },
        { name: 'BlackBerry', value: 'BlackBerry', version: '/' },
        { name: 'Macintosh', value: 'Mac', version: 'OS X' },
        { name: 'Linux', value: 'Linux', version: 'rv' },
        { name: 'Palm', value: 'Palm', version: 'PalmOS' }
    ],
    databrowser: [
        { name: 'Chrome', value: 'Chrome', version: 'Chrome' },
        { name: 'Firefox', value: 'Firefox', version: 'Firefox' },
        { name: 'Safari', value: 'Safari', version: 'Version' },
        { name: 'Internet Explorer', value: 'MSIE', version: 'MSIE' },
        { name: 'Opera', value: 'Opera', version: 'Opera' },
        { name: 'BlackBerry', value: 'CLDC', version: 'CLDC' },
        { name: 'Mozilla', value: 'Mozilla', version: 'Mozilla' }
    ],
    init: function () {
        var agent = this.header.join(' '),
            os = this.matchItem(agent, this.dataos),
            browser = this.matchItem(agent, this.databrowser);

        return { os: os, browser: browser };
    },
    matchItem: function (string, data) {
        var i = 0,
            j = 0,
            html = '',
            regex,
            regexv,
            match,
            matches,
            version;

        for (i = 0; i < data.length; i += 1) {
            regex = new RegExp(data[i].value, 'i');
            match = regex.test(string);
            if (match) {
                regexv = new RegExp(data[i].version + '[- /:;]([\\d._]+)', 'i');
                matches = string.match(regexv);
                version = '';
                if (matches) { if (matches[1]) { matches = matches[1]; } }
                if (matches) {
                    matches = matches.split(/[._]+/);
                    for (j = 0; j < matches.length; j += 1) {
                        if (j === 0) {
                            version += matches[j] + '.';
                        } else {
                            version += matches[j];
                        }
                    }
                } else {
                    version = '0';
                }
                return {
                    name: data[i].name,
                    version: parseFloat(version)
                };
            }
        }
        return { name: 'unknown', version: 0 };
    }
};