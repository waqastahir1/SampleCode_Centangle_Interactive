// In development, always fetch from the network and do not enable offline support.
// This is because caching would make development more difficult (changes would not
// be reflected on the first load after each change).

// Caution! Be sure you understand the caveats before publishing an application with
// of 

async function onFetch(event) {
    let cachedResponse = null;
    if (event.request.method === 'GET') {
        // For all navigation requests, try to serve index.html from cache
        // If you need some URLs to be server-rendered, edit the following check to exclude those URLs
        const shouldServeIndexHtml = event.request.mode === 'navigate'
            && !event.request.url.includes('/connect/')
            && !event.request.url.includes('/Identity/')
            && !event.request.url.includes('/Account/')
            && !event.request.url.includes('/v1/');
         
    }

    return cachedResponse || fetch(event.request);
}
 

self.addEventListener('push', event => {
    const payload = event.data.json();
    event.waitUntil(
        self.registration.showNotification('CIT-Portal', {
            body: payload.message,
            icon: 'assets/media/logos/sos-logo-300x100.png',
            vibrate: [100, 50, 100],
            data: { url: payload.url }
        })
    );

    event.waitUntil(async function () {
        const allClients = await clients.matchAll({
            includeUncontrolled: true
        });

        let chatClient;

        // Let's see if we already have a chat window open:
        for (const client of allClients) {
            const url = new URL(client.url);

            if (url.pathname == '/shipments/0' || url.pathname == '/shipments/1') {
                // Excellent, let's use it!
                client.focus();
                chatClient = client;
                break;
            }
        }

        // Message the client:
        if (chatClient != undefined) {
            chatClient.postMessage(payload.message);
        }
    }());

});


self.addEventListener('notificationclick', event => {
    event.notification.close();
    event.waitUntil(clients.openWindow(event.notification.data.url));
});

