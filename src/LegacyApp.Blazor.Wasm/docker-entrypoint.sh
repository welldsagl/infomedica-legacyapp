#!/bin/sh
set -eu
# L'URL dell'API non e' noto al momento della build (dipende dall'ambiente):
# viene scritto a runtime nel file di configurazione che Blazor carica all'avvio.
if [ -n "${API_BASE_URL:-}" ]; then
  printf '{\n  "ApiBaseUrl": "%s"\n}\n' "$API_BASE_URL" > /srv/appsettings.json
  rm -f /srv/appsettings.json.br /srv/appsettings.json.gz   # versioni precompresse del valore di build
fi
exec caddy run --config /etc/caddy/Caddyfile --adapter caddyfile
