# Spotify Playlist Extractor

Aplicación de consola (.NET) que permite exportar las listas de reproducción de Spotify a archivos de texto plano (.txt).

## Descripción

Esta herramienta extrae automáticamente la información de canciones de cualquier playlist pública de Spotify y genera un archivo de texto con el formato "Canción - Artista", facilitando el uso de estos datos para otros propósitos como:

- Descargar canciones y armar playlists locales
- Crear playlists en otras plataformas de música
- Procesar los datos para análisis o reportes
- Hacer respaldos de tus playlists favoritas

## Características

- Validación automática de URLs de Spotify
- Extracción automática del ID de playlist
- Exportación a archivo .txt con formato legible
- Interfaz de consola simple y directa
- Manejo de errores para URLs inválidas

## Configuración

Es necesario configurar las credenciales de la API de Spotify previamente. Obetener Client ID/Client Secret y configurarlas en un archivo .env.

## Estructura del archivo de salida

```
Nombre de la Canción - Nombre del Artista
Otra Canción - Otro Artista
...
```
