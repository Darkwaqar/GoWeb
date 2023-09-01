// Give the service worker access to Firebase Messaging.
// Note that you can only use Firebase Messaging here, other Firebase libraries
// are not available in the service worker.
importScripts('https://www.gstatic.com/firebasejs/4.8.1/firebase-app.js');
importScripts('https://www.gstatic.com/firebasejs/4.8.1/firebase-messaging.js');

// Initialize the Firebase app in the service worker by passing in the
// messagingSenderId.
var config = {
    apiKey: "AIzaSyDcUaKH22dOCAU6ZBFx35RRmAb4l6-vZc4",
    authDomain: "mobishop-let-s-shop-instyle.firebaseapp.com",
    databaseURL: "https://mobishop-let-s-shop-instyle.firebaseio.com",
    projectId: "mobishop-let-s-shop-instyle",
    storageBucket: "mobishop-let-s-shop-instyle.appspot.com",
    messagingSenderId: "363361383809"
};
firebase.initializeApp(config);


// Retrieve an instance of Firebase Messaging so that it can handle background
// messages.
var messaging = firebase.messaging();

messaging.setBackgroundMessageHandler(function (payload) {
    console.log('[firebase-messaging-sw.js] Received background message ', payload);
    // Customize notification here
    var notificationTitle = payload.data.title;
    var notificationOptions = {
        body: payload.notification.body,
        icon: "https://web-push-book.gauntface.com/images/demos/icon-512x512.png",
        image: payload.data.image,
        badge: payload.data["gcm.notification.badge"],
    };

    return self.registration.showNotification(notificationTitle,
        notificationOptions);
});
