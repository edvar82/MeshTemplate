# Spatial Mesh Visualization in Unity

## Sobre o Projeto
Este projeto demonstra como carregar e visualizar malhas espaciais (Spatial Meshes) no Unity utilizando arquivos OBJ pré-mapeados e o Mixed Reality Toolkit (MRTK). O aplicativo permite carregar uma malha espacial, configurar colisões físicas adequadas e interagir com a malha.

## Requisitos Técnicos
- Unity 2021.3.22f1
- Mixed Reality Toolkit (MRTK) 2.8
- Universal Windows Platform (para HoloLens)

## Funcionalidades Implementadas
- Carregamento de malhas espaciais a partir de arquivos OBJ na pasta Resources
- Configuração automática de colliders para permitir interação física com a malha
- Sistema para evitar atravessar paredes da malha espacial
- Integração com sistema de entrada do MRTK para interação com a malha
- Destaque visual de partes da malha ao interagir com ela
- Chão de segurança para evitar queda do personagem

## Estrutura do Projeto
- `Assets/Resources/SpatialMeshes/` - Arquivos OBJ das malhas espaciais
- `Assets/Scripts/` - Scripts C# para carregar e interagir com a malha
- `Assets/Materials/` - Materiais para visualização da malha

## Scripts Principais

### SpatialMeshLoader.cs
Responsável por carregar e configurar a malha espacial:
- Carrega o arquivo OBJ da pasta Resources
- Configura materiais para visualização
- Adiciona e configura colliders para interação física
- Posiciona a malha em frente ao personagem

### CollisionLayerManager.cs
Gerencia as configurações de colisão entre layers:
- Força colisões entre a layer Spatial e Default (do jogador)
- Garante que as configurações persistam mesmo com MRTK ativo

### SpatialMeshInteraction.cs
Gerencia interações do usuário com a malha:
- Detecta quando o usuário aponta/toca na malha
- Aplica material de destaque às partes selecionadas
- Integra com o sistema de input do MRTK

## Como Configurar o Projeto

1. **Configuração Inicial:**
   - Criar um novo projeto Unity 2021.3.22f1
   - Instalar o MRTK 2.8 via Package Manager
   - Configurar XR Plug-in Management para OpenXR ou Windows MR

2. **Importação de Malhas:**
   - Criar pasta `Assets/Resources/SpatialMeshes/`
   - Importar arquivos OBJ para esta pasta
   - Configurar as importações com "Generate Colliders" e "Read/Write Enabled"

3. **Configuração da Cena:**
   - Adicionar MRTK à cena usando "Mixed Reality > Toolkit > Add to Scene"
   - Criar GameObject vazio para "SpatialMeshManager"
   - Adicionar o script SpatialMeshLoader a este objeto
   - Criar GameObject vazio para "CollisionManager" 
   - Adicionar o script CollisionLayerManager a este objeto

4. **Configuração do Script Loader:**
   - No Inspector do SpatialMeshLoader, definir:
     - Mesh Object Path: "SpatialMeshes/SeuArquivoOBJ" (sem extensão)
     - Criar e atribuir material para Spatial Mesh Material
     - Marcar Display Mesh e Enable Colliders como true

5. **Configuração de Colisão:**
   - Criar layer "Spatial" nas configurações do projeto
   - Adicionar CharacterController à câmera principal para física adequada
