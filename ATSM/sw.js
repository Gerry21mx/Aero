importScripts('/Ext/pouchdb/dist/pouchdb.min.js');
importScripts('/cjs/sw-db.js');
importScripts('/cjs/sw-utils.js');

const STATIC_CACHE = 'static-v1';
const DYNAMIC_CACHE = 'dynamic-v1';
const INMUTABLE_CACHE = 'inmutable-v1';
const API_CACHE = 'API-v1';

const APP_SHELL = [
    '/',
    '/Content/site.css',
    '/bundles/base?v=Nn2SVZloAuru4VhTpcTAmjsIFddx_paLSTcaX635ERA1',
    '/cjs/sw-utils.js',
    '/cjs/app.js'
];
const APP_SHELL_INMUTABLE = [
    '/Content/timeline?v=nvpw-ZvKCjAw6203DKhdphQ-wnL9qf6OMFvIz3yty6U1',
    '/Content/css?v=mdN6wM2zWPUOIuJnMDLYmayvQ826BTQBMGZpQvibtR01',
    '/Content/datatables?v=uYqJyisK1chC8ok-CK0fLWP45cUZFH0HuWITTE14ECI1',
    '/bundles/jquery?v=235DeCRyc2KKtm5-u-WowaGmWHU3ft44y448NxHpMf41',
    '/bundles/signalr?v=cjoi_M1bmvhHCrKYPsv3qtk0HbuNR47G58GFezIsQAw1',
    '/bundles/timeline?v=0jwSsMveUngV_K3bTjSblr1xAbhI7Z1JbSr3v8usC-w1',
    '/bundles/modernizr?v=inCVuEFe6J4Q07A0AcRsbJic_UE5MwpRMNGcOtk94TE1',
    '/bundles/bootstrap?v=Rc0g6GDBha0U2Ue5txisObdTUK1i4R1sOhOsbTe7GyM1',
    '/bundles/datatables?v=MZvP9eWURxZa0UNmHHQiFZuD42rBq6wPs36_7zuHMSE1',
    '/bundles/plugins?v=V3nDvZTAmsmrrqX9_Cq0Xq6dm6q2XKypdYBj4_hYtEY1',
    '/bundles/bootnavbar?v=G6FfklufZLbsG9z86ER6p69qNbWnKaRpnNR3mXWaNMg1',
    'https://use.fontawesome.com/releases/v5.8.2/css/all.css',
    '/Ext/DataTables/DataTables-1.10.24/images/sort_asc.png',
    '/Ext/DataTables/DataTables-1.10.24/images/sort_asc_disabled.png',
    '/Ext/DataTables/DataTables-1.10.24/images/sort_both.png',
    '/Ext/DataTables/DataTables-1.10.24/images/sort_desc.png',
    '/Ext/DataTables/DataTables-1.10.24/images/sort_desc_disabled.png'

];
self.addEventListener('install', e => {
    const cacheStatic = caches.open(STATIC_CACHE).then(cache =>
        cache.addAll(APP_SHELL));
    const cacheInmutable = caches.open(INMUTABLE_CACHE).then(cache =>
        cache.addAll(APP_SHELL_INMUTABLE));
    e.waitUntil(Promise.all([cacheStatic, cacheInmutable]));
});
self.addEventListener('activate', e => {
    const respuesta = caches.keys().then(keys => {
        keys.forEach(key => {
            if (key !== STATIC_CACHE && key.includes('static')) {
                return caches.delete(key);
            }
            if (key !== DYNAMIC_CACHE && key.includes('dynamic')) {
                return caches.delete(key);
            }
        });
    });
    e.waitUntil(respuesta);
});
self.addEventListener('fetch', e => {
    let respuesta;
    if (e.request.url.includes('/api/')) {
        // return respuesta????
        //respuesta = manejoApiMensajes(DYNAMIC_CACHE, e.request);
        respuesta = manejoApiMensajes(API_CACHE, e.request);
    } else {
        respuesta = caches.match(e.request).then(res => {
            if (res) {
                actualizaCacheStatico(STATIC_CACHE, e.request, APP_SHELL_INMUTABLE);
                return res;
            } else {
                return fetch(e.request).then(newRes => {
                    return actualizaCacheDinamico(DYNAMIC_CACHE, e.request, newRes);
                });
            }
        });
    }
    e.respondWith(respuesta);
});
// tareas asíncronas
self.addEventListener('sync', e => {
    console.log('SW: Sync');
    if (e.tag === 'nuevo-post') {
        // postear a BD cuando hay conexión
        const respuesta = postearMensajes();
        e.waitUntil(respuesta);
    }
});