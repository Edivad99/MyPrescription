(function () {
    window.blazorPushNotifications = {
        requestSubscription: async (publicKeyUrl) => {
            const worker = await navigator.serviceWorker.getRegistration();
            const existingSubscription = await worker.pushManager.getSubscription();
            if (!existingSubscription) {
                const newSubscription = await subscribe(worker, publicKeyUrl);
                if (newSubscription) {
                    return {
                        url: newSubscription.endpoint,
                        p256dh: arrayBufferToBase64(newSubscription.getKey('p256dh')),
                        auth: arrayBufferToBase64(newSubscription.getKey('auth'))
                    };
                }
            }
        },
        checkNotificationPermission: async () => {
            return Notification.permission === "granted";
        },
        askNotificationPermission: async () => {
            Notification.requestPermission().then(res => { });
        }
    };

    async function getServerPublicKey(publicKeyUrl) {
        const publicKey = await fetch(publicKeyUrl).then(res => res.text());
        return publicKey;
    }

    async function subscribe(worker, publicKeyUrl) {
        try {
            applicationServerPublicKey = await getServerPublicKey(publicKeyUrl);
            return await worker.pushManager.subscribe({
                userVisibleOnly: true,
                applicationServerKey: applicationServerPublicKey
            });
        } catch (error) {
            if (error.name === 'NotAllowedError') {
                return null;
            }
            throw error;
        }
    }

    function arrayBufferToBase64(buffer) {
        // https://stackoverflow.com/a/9458996
        var binary = '';
        var bytes = new Uint8Array(buffer);
        var len = bytes.byteLength;
        for (var i = 0; i < len; i++) {
            binary += String.fromCharCode(bytes[i]);
        }
        return window.btoa(binary);
    }
})();
