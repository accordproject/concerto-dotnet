 import {
  Factory,
  ModelManager,
  ModelFile,
  ModelUtil,
  Serializer,
} from "@accordproject/concerto-core";
import { ValidationResult } from "./validation-result";
import { ValidationOptions } from "./validation-options";

export function validateInstance (
  callback: (error: Error|string, result: ValidationResult) => any,
  jSonModels: string,
  instanceJson: string,
  options: ValidationOptions = {strict: true, enableMapType: true}
): void{
  const models = JSON.parse(jSonModels) as any[];
  const modelManager = new ModelManager({ strict: options.strict, enableMapType: options.enableMapType });
  const modelFiles = models.map((item) => new ModelFile(modelManager, item));
  modelFiles.sort((a, b) => a.getNamespace().localeCompare(b.getNamespace()));
  modelManager.addModelFiles(modelFiles);
  
  try {
    const factory = new Factory(modelManager);
    const serializer = new Serializer(factory, modelManager);
    const instance = JSON.parse(instanceJson)
    const resource = serializer.fromJSON(instance);
    callback(null, 
    {
      instance: JSON.stringify(serializer.toJSON(resource)),
      id: resource.isIdentifiable() ? resource.getIdentifier() : undefined,
      isValid: true,
    });
  } catch (error) {
    
    callback(null, 
      {
        isValid: false,
        errorMessage: error.message,
      });
    
    return 
  }
}

export function getAllReferencedFqns(
  callback: (error: Error|string, fqns: string[]) => any,
  objectJson: string): void {
  callback(null, getAllReferencedFqnsImpl(objectJson));
}

export function getAllNeededNamespaces(
  callback: (error: Error|string, namespaces: string[]) => any,
  objectJson: string): void {
    const fqns = getAllReferencedFqnsImpl(objectJson);
    const namespaceSet = new Set<string>();
    fqns.forEach( fqn => {
      const parsedFqn = parseFullyQualifiedName(fqn);
      namespaceSet.add( `${parsedFqn[0]}.${parsedFqn[1]}`);
    })
    callback(null, Array.from(namespaceSet));
  }
  
  function parseFullyQualifiedName(fqn: string): [modelName: string, versionName: string | undefined, typeName: string] {
    try {
      const typeName = ModelUtil.getShortName(fqn);
      const namespace = ModelUtil.getNamespace(fqn);
      const { name: modelName, version: versionName } = ModelUtil.parseNamespace(namespace);
      if (!typeName) {
        throw new Error('Missing type name');
      } else if (!modelName) {
        throw new Error('Missing model name');
      } else if (!versionName) {
        throw new Error('Missing version name');
      }
      return [modelName, versionName, typeName];
    } catch (error) {
      throw new Error(`Failed to parse fully qualified name "${fqn}": ${error.message}`);
    }
  }

function getAllReferencedFqnsImpl(objectJson: string): string[] {
  const objectStack: any[] = [JSON.parse(objectJson)];
  const referenceSet = new Set<string>();
  
  do {
    const workingObject = objectStack.pop();
  
    Object.entries(workingObject).forEach(([key, value]) => {
      if (key === '$class' && typeof value === 'string') {
        referenceSet.add(value as string);
      } else if (typeof value === 'object') {
        objectStack.push(value);
      }
    });
  } while (objectStack.length > 0);
  return Array.from(referenceSet);
}
