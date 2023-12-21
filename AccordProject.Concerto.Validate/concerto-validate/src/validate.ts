 import {
  Factory,
  ModelManager,
  ModelFile,
  Serializer,
} from "@accordproject/concerto-core";

export interface ValidationResult {
  instance?: string;
  id?: string;
  isValid: boolean;
  errorMessage?: string;
}

export function validateInstance (
  callback: (error: Error|string, result: ValidationResult) => any,
  jSonModels: string,
  instanceJson: string
): void{
  const models = JSON.parse(jSonModels) as any[];
  const modelManager = new ModelManager({ strict: true });
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
