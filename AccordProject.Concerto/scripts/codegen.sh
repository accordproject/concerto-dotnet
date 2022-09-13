#!/usr/bin/env bash
set -euo pipefail
if [ -d output ]; then
    rm -rf output
fi
npx @accordproject/concerto-cli@unstable compile --metamodel --target CSharp --strict --useNewtonsoftJson
cat scripts/header.txt output/concerto@1.0.0.cs > ConcertoTypes.cs
cat scripts/header.txt output/concerto.metamodel@1.0.0.cs > ConcertoMetamodelTypes.cs
rm -rf output