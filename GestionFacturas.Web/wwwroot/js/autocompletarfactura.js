// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function autocompleteJqueryUiCliente(idCampo, urlAutocompletar) {
    $("#" + idCampo).autocomplete({
        source: urlAutocompletar,
        messages: {
            noResults: '',
            results: function () { }
        },
        select: function (event, ui) {
            completarDatosCliente(ui);
            return false;
        }
    }).autocomplete("instance")._renderItem = function (ul, item) {
        return $("<li>")
            .append("<a>" + item.Id + " - " + item.NombreOEmpresa + "<br>" + item.NumeroIdentificacionFiscal + "</a>")
            .appendTo(ul);
    };
}

function completarDatosCliente(ui) {
    $("#Editor_IdComprador").val(ui.item.Id);
    $("#Editor_CompradorNombreOEmpresa").val(ui.item.NombreOEmpresa);
    $("#Editor_CompradorNumeroIdentificacionFiscal").val(ui.item.NumeroIdentificacionFiscal);
    $("#Editor_CompradorDireccion1").val(ui.item.Direccion1);
    $("#Editor_CompradorDireccion2").val(ui.item.Direccion2);
    $("#Editor_CompradorLocalidad").val(ui.item.Localidad);
    $("#Editor_CompradorProvincia").val(ui.item.Provincia);
    $("#Editor_CompradorCodigoPostal").val(ui.item.CodigoPostal);
    $("#Editor_CompradorCodigoPaisIso").val(ui.item.CodigoPaisIso2);
    $("#Editor_CompradorEmail").val(ui.item.Email);
}


function configurarInputsAutocomplete(
    autocompletarPorIdUrl,
    autocompletarPorNombreUrl,
    autocompletarPorIdentificacionFiscalUrl) {

    autocompleteJqueryUiCliente("Editor_CompradorNombreOEmpresa", autocompletarPorNombreUrl);
    autocompleteJqueryUiCliente("Editor_IdComprador", autocompletarPorIdUrl);
    autocompleteJqueryUiCliente("Editor_CompradorNumeroIdentificacionFiscal", autocompletarPorIdentificacionFiscalUrl);
}
