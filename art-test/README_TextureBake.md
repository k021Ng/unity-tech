## Blender
1. UVEditing 创建智能 UV  
2. 打开 Shading，选择任意 Material，添加纹理节点，新建图片用于 Bake，最好将这个编为组，方便统一修改，然后拷贝这个组到该物品的所有 mateirals 下  
3. 选择渲染属性，渲染引擎选择 cycles，点击烘焙，当然可能在渲染窗口看到的表现和渲染的图片不一致，可以修改下面的色彩管理和上面图片的色彩空间，保存 bake 图片  
4. 合并物品的材质，Shift D 对象，然后创建一个 纹理材质给他，导出 fbx  
5. 将 fbx 和 bake 图片导入 unity  

## Unity
1. 颜色空间修改为 linear  
2. 去掉灯光  
3. 修改 Lighting 下面的 Environment 设置，去掉灯光，将颜色设置为白色  

## 问题
1. blender 渲染出来的图片与渲染窗口还是不一致（太暗了）  
2. unity 里面的效果与 blender 也不一致  