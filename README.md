# Spotify Playlist Extractor

Aplicación de consola (.NET) que permite exportar las listas de reproducción de Spotify a archivos de texto plano (.txt).
Esta herramienta extrae automáticamente la información de canciones de cualquier playlist pública de Spotify y genera un archivo de texto, facilitando el uso de estos datos para otros propósitos.

## Usos posibles

- Descargar canciones y armar playlists locales
- Crear playlists en otras plataformas de música
- Procesar los datos para análisis o reportes
- Simplemente generar un respaldo de tus playlists favoritas

## Características

- Validación automática de URLs de Spotify
- Extracción automática del ID de playlist
- Exportación a archivo .txt con formato legible
- Interfaz de consola simple y directa
- Manejo de errores para URLs inválidas

## Configuración

Es necesario configurar las credenciales de la API de Spotify previamente. Obetener Client ID/Client Secret y configurarlas en un archivo .env.

## Ejemplo parcial de archivo de salida

```
Pages - O.C.O.E. (Official Cat Of The Eighties)
Michael McDonald - I Keep Forgettin' (Every Time You're Near)
Ambrosia - Biggest Part of Me
The Doobie Brothers - What a Fool Believes
Kenny Loggins - This Is It
Steely Dan - Hey Nineteen
TOTO - Rosanna
Gilberto Gil - Tempo Rei
Boz Scaggs - Lowdown
...
```
