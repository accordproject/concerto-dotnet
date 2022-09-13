#!/usr/bin/env bash
set -euo pipefail
if [ -d output ]; then
    rm -rf output
fi
npx --yes @accordproject/concerto-cli@unstable parse --model data/patent.cto --output data/patent.json
npx --yes @accordproject/concerto-cli@unstable compile --model data/employee.cto --target CSharp --strict --useNewtonsoftJson
cat scripts/header.txt output/org.accordproject.concerto.test@1.2.3.cs > TestTypes.cs
rm -rf output