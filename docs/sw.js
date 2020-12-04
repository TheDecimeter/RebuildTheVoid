var cacheName = 'RebuildTheVoid-V3.0';
var filesToCache = [
  'index.html',
  
  'TemplateData/favicon.ico',
  'TemplateData/icon-192.png',
  'TemplateData/icon-256.png',
  'TemplateData/icon-512.png',
  'TemplateData/favicon.ico',
  'TemplateData/fullscreen.png',
  'TemplateData/progressEmpty.Dark.png',
  'TemplateData/progressEmpty.Light.png',
  'TemplateData/progressFull.Dark.png',
  'TemplateData/progressFull.Light.png',
  'TemplateData/progressLogo.Dark.png',
  'TemplateData/progressLogo.Light.png',
  'TemplateData/style.css',
  'TemplateData/UnityProgress.js',
  'TemplateData/webgl-logo.png',
  
  'Build/RebuildTheVoidHTML.data.unityweb',
  'Build/RebuildTheVoidHTML.json',
  'Build/RebuildTheVoidHTML.wasm.code.unityweb',
  'Build/RebuildTheVoidHTML.wasm.framework.unityweb',
  'Build/UnityLoader.js'
];
 
 //install cache
self.addEventListener('install', function(event) {
  console.log('sw install');
  event.waitUntil(
    caches.open(cacheName).then(function(cache) {
      console.log('sw caching files');
      return cache.addAll(filesToCache);
    }).catch(function(err) {
      console.error("install error "+err);
    })
  );
});

//fetch cache when needed
self.addEventListener('fetch', (event) => {
  console.log('sw fetch ',event.request.url);
  event.respondWith(
    caches.match(event.request).then(function(response) {
      return response || fetch(event.request);
    }).catch(function (error) {
      console.error("fetch error "+error);
    })
  );
});

//keep cache relevant
self.addEventListener('activate', function(event) {
  console.log('sw activate');
  event.waitUntil(
    caches.keys().then(function(keyList) {
      return Promise.all(keyList.map(function(key) {
        if (key !== cacheName) {
          console.log('sw removing old cache', key);
          return caches.delete(key);
        }
      }));
    })
  );
});