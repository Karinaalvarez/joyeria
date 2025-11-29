// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Funciones de ayuda para el sitio
$(document).ready(function () {
    console.log("El sitio está cargado y listo.");
    
    // Depuración de formularios
    if ($('form').length > 0) {
        console.log("Se detectaron formularios en la página.");
        
        // Interceptar envío de formularios para depuración
        $('form').on('submit', function(e) {
            console.log("Formulario enviado");
            console.log("Valores del formulario:", $(this).serialize());
            
            // Verificar validez del formulario
            if ($(this).valid) {
                const isValid = $(this).valid();
                console.log("¿El formulario es válido?", isValid);
                
                if (!isValid) {
                    console.log("Errores de validación:", $(this).validate().errorList);
                    e.preventDefault(); // Evitar envío si no es válido
                }
            }
        });
    }
});
