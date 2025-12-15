# SysClin_ProyectoFinal
Proyecto final del curso **Pruebas de Aseguramiento de la Calidad del Software**.

## Descripción general

Este repositorio corresponde al proyecto final del curso Pruebas de Aseguramiento de la Calidad del Software. El objetivo del proyecto es evidenciar el diseño, automatización y ejecución de pruebas unitarias y de integración sobre el sistema **SysClin**, una plataforma web para la gestión de citas médicas.

SysClin es un sistema desarrollado en ASP.NET MVC que permite el registro y autenticación de usuarios, la gestión de servicios médicos y la creación de citas por parte de los pacientes. A partir de este sistema se implementaron pruebas automatizadas para validar reglas de negocio, validaciones de datos y flujos críticos, demostrando la detección de errores y su posterior corrección mediante pruebas.

## Estructura del repositorio

El repositorio se encuentra organizado de la siguiente manera:

- **Version_A_Codigo_Con_Errores**  
  Contiene la versión del sistema con errores intencionales, utilizada para demostrar la ejecución de pruebas automatizadas que fallan al detectar problemas en el código.

- **Version_B_Codigo_Corregido**  
  Contiene la versión corregida del sistema, en la cual todas las pruebas automatizadas se ejecutan de forma exitosa.

- **Database**  
  Incluye los scripts SQL necesarios para la creación de tablas y la inserción de datos iniciales requeridos por el sistema.

## Pruebas implementadas

El proyecto incluye pruebas unitarias y pruebas de integración. Las pruebas unitarias validan funcionalidades relacionadas con:

- Registro de usuarios
- Inicio de sesión
- Perfil profesional
- Gestión de servicios
- Creación de citas
- Utilidades como el manejo de contraseñas

Adicionalmente, se implementan pruebas de integración para el proceso de autenticación, debido a la interacción entre distintos componentes del sistema.

Todas las pruebas fueron desarrolladas utilizando el framework **xUnit**.

## Ejecución del proyecto

Para ejecutar el sistema:

1. Ingresar a la carpeta correspondiente a la versión deseada del código.
2. Abrir el archivo **SysClin.sln** utilizando Visual Studio.

En algunos casos, Visual Studio puede intentar ejecutar el proyecto de pruebas como proyecto de inicio. Si esto ocurre, se debe seleccionar el proyecto **SysClin**, hacer clic derecho y elegir la opción **Establecer como proyecto de inicio**. Posteriormente, el sistema se puede ejecutar con normalidad.

## Ejecución de las pruebas

Para ejecutar las pruebas automatizadas del proyecto:

1. Desde Visual Studio, acceder al menú **Compilar** y seleccionar la opción **Limpiar solución**.
2. Una vez finalizado el proceso de limpieza, seleccionar **Compilar solución** desde el mismo menú.
3. Acceder al menú **Prueba** y abrir el **Explorador de pruebas**.
4. Ejecutar todas las pruebas utilizando la opción **Ejecutar**.

Los resultados de la ejecución se visualizan directamente en el Explorador de pruebas, permitiendo identificar de forma clara los casos que fallan y los que se ejecutan correctamente según la versión del código analizada. No es necesario ejecutar la aplicación web para realizar la ejecución de las pruebas.

## Base de datos

La carpeta **Database** contiene los scripts necesarios para la creación de la base de datos y la carga de datos iniciales del sistema. Para ejecutar el proyecto es necesario configurar el *connection string* correspondiente en el archivo **Web.config**.

## Tecnologías utilizadas

- C#
- ASP.NET MVC 5
- xUnit
- SQL Server
- Visual Studio 2022
- GitHub

## Contexto académico

Este proyecto fue desarrollado como parte del proyecto final del curso **Pruebas de Aseguramiento de la Calidad del Software**, con el objetivo de aplicar principios de aseguramiento de la calidad, automatización de pruebas y documentación de resultados.
