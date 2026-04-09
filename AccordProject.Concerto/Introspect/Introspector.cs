/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace AccordProject.Concerto;

/// <summary>
/// Provides a query surface over the declarations registered in a ModelManager.
/// Mirrors Introspector in concerto-core.
/// </summary>
public sealed class Introspector
{
    private readonly ModelManager modelManager;

    public Introspector(ModelManager modelManager)
    {
        this.modelManager = modelManager ?? throw new ArgumentNullException(nameof(modelManager));
    }

    public IReadOnlyCollection<ClassDeclaration> GetClassDeclarations()
        => modelManager.GetClassDeclarations();

    public ClassDeclaration GetClassDeclaration(string fullyQualifiedTypeName)
        => modelManager.GetType(fullyQualifiedTypeName)
            ?? throw new KeyNotFoundException($"Unknown class declaration '{fullyQualifiedTypeName}'.");
}
