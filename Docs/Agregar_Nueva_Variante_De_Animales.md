# Sistema de Variantes Visuales de Animales

Este documento detalla el funcionamiento técnico del sistema de variantes estéticas para los animales del refugio y provee una guía paso a paso para que artistas y programadores agreguen nuevas apariencias (skins/variantes) al juego sin romper la lógica existente.

---

## 🛠️ Arquitectura del Sistema

El sistema utiliza un enfoque de **Data-Driven & Separación de Datos/Visuales**:

1. **`AnimalData`**: Estructura pura que almacena los datos básicos, estadísticas del animal y su respectivo `variantIndex` (un entero único generado al azar).
2. **`AnimalGenerator.cs`**: Clase estática encargada de la creación de la data. Consulta dinámicamente al catálogo de prefabs para saber cuántas variantes existen de una especie concreta mediante `catalog.GetVariantCount(species)` y elige una al azar dentro de ese rango `[0, totalVariants)`.
3. **`AnimalPrefabCatalog.cs`**: ScriptableObject que centraliza las referencias a los prefabs base de cada especie (`Dog.prefab`, `Cat.prefab`, etc.).
4. **`Animal.cs`**: Componente adjunto al prefab del animal. Contiene un array de GameObjects (`visualVariants`). Al inicializarse con un `AnimalData`, desactiva todas las variantes por defecto y **activa exclusivamente** el GameObject hijo correspondiente al índice asignado.

---

## 📝 Guía Paso a Paso: Cómo agregar una nueva variante visual

Para incorporar un nuevo aspecto visual a una especie existente (por ejemplo, un nuevo tipo de perro o gato), sigue rigurosamente los siguientes pasos en el editor de Unity:

### Paso 1: Analizar las Variantes Existentes (Referencia Obligatoria)

Antes de crear cualquier objeto, abre el prefab del animal (ej. `Dog.prefab`) y examina cómo están configuradas las variantes por defecto (`Variant_0`, `Variant_1`).

- Observa la estructura de componentes que tienen asignados.
- Revisa qué **Animation Controller** tienen asociado en su componente `Animator` para asegurar que las animaciones de idle, movimiento o interacción mantengan los mismos nombres de parámetros.

### Paso 2: Preparación del Arte y Animaciones

1. Importa tus Sprites e integra las animaciones correspondientes en el proyecto.
2. Crea un nuevo **Animation Controller** específico para la variante si los tiempos o los sprites cambian, o prepara los estados respetando la misma estructura interna que viste en el paso de análisis.

### Paso 3: Modificación de la Jerarquía en el Prefab

1. Localiza el prefab de la especie en tu carpeta de proyecto y ábrelo en el **Prefab Mode**.
2. Despliega la jerarquía del objeto. Verás una estructura idéntica a esta:

```text
      [Nombre_Especie] (ej: Dog)
      └── AnimalNeedsUI
      └── VisualVariants
           ├── Variant_0  <-- (Activo/Inactivo por código)
           ├── Variant_1
           └── Variant_New <-- [Tu nuevo GameObject aquí]
```

3. Crea un nuevo GameObject hijo dentro del contenedor de variantes (`VisualVariants`) y asígnale un nombre descriptivo siguiendo la nomenclatura del proyecto (ej: `Variant_2` o `Variant_Marmolado`).
4. Añade a este nuevo GameObject los componentes esenciales copiando los valores analizados en el Paso 1:
   - **`SpriteRenderer`**: Configura el sprite por defecto de la variante.
   - **`Animator`**: Asigna el Animation Controller que preparaste en el _Paso 2_.

⚠️ **Muy Importante:** Asegúrate de **desactivar** el nuevo GameObject en el inspector (desmarca el checkbox al lado del nombre del objeto) tal y como están las variantes que tomaste de referencia. El script `Animal.cs` se encargará de activarlo cuando corresponda en tiempo de ejecución. Dejarlo activo causará que se superponga visualmente con otras variantes en el corral.

### Paso 4: Registro en el Script Base del Animal

1. Selecciona el objeto raíz del Prefab (el objeto padre que posee el componente principal `Animal`).
2. En el componente `Animal`, busca la sección **Visual Variants** en el Inspector.
3. Incrementa el tamaño (`Size`) de la lista/array agregando un nuevo casillero.
4. Arrastra tu nuevo GameObject creado en el _Paso 3_ al espacio vacío (`Element X`) recién generado.
5. Guarda y aplica los cambios en el Prefab (`Save / Override`).

⚠️ **Importante:** Si no realizas este paso, el sistema no reconocerá la nueva variante y no podrá activarla en tiempo de ejecución.

---

## 🔍 Verificación y Flujo Automático

¡Listo! No necesitas realizar ningún cambio en código. El flujo se actualizará de la siguiente manera:

1. El `AnimalPrefabCatalog` leerá automáticamente el nuevo tamaño del array `visualVariants` de tu prefab.
2. `AnimalGenerator` detectará el incremento de posibilidades de inmediato y empezará a sortear el nuevo número de índice de variante asignado entre los animales rescatados del día.
