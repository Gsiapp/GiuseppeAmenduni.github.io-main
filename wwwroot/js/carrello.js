function aggiungiAlCarrello(prodottoId, quantita = 1) {
    $.ajax({
        url: '/Carrello/AggiungiAlCarrello',
        type: 'POST',
        data: { prodottoId: prodottoId, quantita: quantita },
        success: function(response) {
            if (response.success) {
                aggiornaContatoreCarrello();
                mostraNotifica('success', response.message);
            } else {
                mostraNotifica('error', response.message);
            }
        },
        error: function() {
            mostraNotifica('error', 'Si è verificato un errore durante l\'aggiunta al carrello');
        }
    });
}

function rimuoviDalCarrello(prodottoId) {
    $.ajax({
        url: '/Carrello/RimuoviDalCarrello',
        type: 'POST',
        data: { prodottoId: prodottoId },
        success: function(response) {
            if (response.success) {
                aggiornaContatoreCarrello();
                mostraNotifica('success', response.message);
                // Rimuovi l'elemento dalla vista
                $(`#carrello-item-${prodottoId}`).remove();
            } else {
                mostraNotifica('error', response.message);
            }
        },
        error: function() {
            mostraNotifica('error', 'Si è verificato un errore durante la rimozione dal carrello');
        }
    });
}

function aggiornaQuantita(prodottoId, quantita) {
    $.ajax({
        url: '/Carrello/AggiornaQuantita',
        type: 'POST',
        data: { prodottoId: prodottoId, quantita: quantita },
        success: function(response) {
            if (response.success) {
                aggiornaContatoreCarrello();
                mostraNotifica('success', response.message);
            } else {
                mostraNotifica('error', response.message);
            }
        },
        error: function() {
            mostraNotifica('error', 'Si è verificato un errore durante l\'aggiornamento della quantità');
        }
    });
}

function aggiornaContatoreCarrello() {
    $.get('/Carrello/GetContatore', function(count) {
        $('#carrello-contatore').text(count);
    });
}

function mostraNotifica(tipo, messaggio) {
    const toast = $(`
        <div class="toast align-items-center text-white bg-${tipo} border-0" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="d-flex">
                <div class="toast-body">
                    ${messaggio}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
            </div>
        </div>
    `);
    
    $('#toast-container').append(toast);
    const bsToast = new bootstrap.Toast(toast);
    bsToast.show();
    
    toast.on('hidden.bs.toast', function() {
        toast.remove();
    });
} 