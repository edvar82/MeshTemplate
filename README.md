# Spatial Mesh Visualizer

## Sobre o Projeto
Este projeto demonstra como carregar e visualizar malhas espaciais (Spatial Meshes) no Unity utilizando o Mixed Reality Toolkit (MRTK). O aplicativo permite visualizar malhas espaciais pré-mapeadas (arquivos OBJ).

## Requisitos Técnicos
- Unity 2021.3.22f1
- Mixed Reality Toolkit (MRTK) 2.8.3.0
- Universal Windows Platform (para HoloLens)
- O projeto utiliza Universal Render Pipeline (URP)

## Funcionalidades
- Carregamento e visualização de malhas espaciais a partir de arquivos OBJ
- Destaque visual das partes da malha

## Instruções de Instalação
1. Clone este repositório
2. Abra o projeto no Unity 2021.3.22f1
3. Certifique-se de que o MRTK 2.8.3.0 está instalado
4. Abra a cena principal em `Assets/Scenes/Main.scene`

## Como Usar
1. Execute o aplicativo no editor ou em um dispositivo HoloLens
2. A malha espacial será carregada automaticamente na frente da câmera
3. Navegue pelo ambiente usando os controles de movimento padrão

## Estrutura do Projeto
- `Assets/Prefabs/` - Contém o prefab da malha espacial
- `Assets/Materials/` - Materiais utilizados para visualizar a malha
- `Assets/Scripts/` - Scripts C# para carregar e interagir com a malha
- `Assets/SpatialMeshes/` - Arquivos OBJ das malhas espaciais

## Scripts Principais
- `SpatialMeshLoader.cs` - Carrega e gerencia a malha espacial
- `SpatialMeshInteraction.cs` - Gerencia interações do usuário com a malha [Not working]
- `MeshInteractionHandler.cs` - Processa eventos de interação em partes específicas da malha

## Resolução de Problemas
- Se a malha não aparecer no campo de visão inicialmente, tente girar a câmera ou reiniciar a aplicação

## Como Adicionar Novos Arquivos OBJ para Teste

### Importando Novos Modelos
1. Coloque seu arquivo .obj na pasta `Assets/SpatialMeshes/`
2. No Unity, selecione o arquivo importado
3. No Inspector, configure:
   - **Generate Colliders**: Ativado
   - **Read/Write Enabled**: Ativado
   - **Mesh Compression**: Baixa ou Média
   - **Optimize Mesh**: Ativado
   - **Import Normals**: Calculado
   - Clique em "Apply" para salvar as configurações

### Criando um Prefab
1. Arraste o modelo importado para a cena
2. Ajuste posição, rotação e escala conforme necessário
3. Crie um Empty GameObject e nomeie como "NomeDaSuaMalha_Container"
4. Arraste o modelo como filho deste container
5. Arraste o container da hierarquia para a pasta `Assets/Prefabs/`
6. Isto criará um prefab do seu novo modelo

### Usando o Novo Modelo
1. Selecione o objeto "SpatialMeshManager" na hierarquia
2. No Inspector, localize o componente "SpatialMeshLoader"
3. Arraste seu novo prefab para o campo "Spatial Mesh Prefab"
4. Execute a aplicação para ver o novo modelo carregado

### Formatos de Arquivo Suportados
Além de .obj, você pode usar outros formatos compatíveis com Unity:
- .fbx 
- .3ds