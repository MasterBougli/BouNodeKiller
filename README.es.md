# BouNodeKiller

BouNodeKiller es una utilidad para Windows que ayuda a localizar procesos de Node.js, entender qué están ejecutando y cerrar uno, varios o todos cuando haga falta.

## Enlaces rápidos

- [Français](README.fr.md)
- [English](README.en.md)
- [Repositorio GitHub](https://github.com/MasterBougli/BouNodeKiller)
- [Donar via Streamlabs](https://streamlabs.com/bouglitv)

## Funciones

- lista procesos `node.exe` y `nodejs.exe`
- muestra la línea de comandos completa
- identifica el script o destino que se está ejecutando
- muestra el usuario, el PID, el proceso padre y la hora de inicio
- muestra el directorio de trabajo detectado al arrancar
- permite cerrar el proceso seleccionado
- permite cerrar todos los procesos de Node a la vez

## Estado del proyecto

- aplicación de escritorio Windows basada en WPF
- versión actual: `1.0.4`
- CI de GitHub Actions en cada push y pull request
- compilación automática de release para etiquetas que empiezan con `v`

## Publicación

Las releases se publican automáticamente desde etiquetas `v*`.

Ejemplo:

```bash
git tag v1.0.4
git push origin v1.0.4
```

## Dentro de la app

La interfaz incluye:

- una tabla de procesos con búsqueda
- una columna para el directorio de inicio
- una etiqueta visible de versión
- una ventana "Acerca de" con enlaces al repositorio y a las releases

