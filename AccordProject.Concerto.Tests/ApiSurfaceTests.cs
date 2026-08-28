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

namespace AccordProject.Concerto.Tests;

using System.Text.Json.Serialization;

public class ApiSurfaceTests
{
    [Fact]
    public void NewtonsoftConverterIsDiscoverableFromCoreNamespace()
    {
        var converter = new ConcertoConverterNewtonsoft();

        Assert.IsType<ConcertoConverterNewtonsoft>(converter);
        Assert.IsType<Newtonsoft.Json.JsonConverter>(converter, exactMatch: false);
    }

    [Fact]
    public void SystemTextJsonConverterIsDiscoverableFromCoreNamespace()
    {
        var converterFactory = new ConcertoConverterFactorySystem();

        Assert.IsType<ConcertoConverterFactorySystem>(converterFactory);
        Assert.IsType<JsonConverterFactory>(converterFactory, exactMatch: false);
    }
}
