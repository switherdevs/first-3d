from pathlib import Path
import numpy as np
import trimesh
from shapely.geometry import Point, Polygon, box
from shapely.affinity import scale
from PIL import Image, ImageDraw, ImageFilter
import matplotlib
matplotlib.use('Agg')
import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d.art3d import Poly3DCollection
import zipfile, json
out=Path(__file__).parent
# Millimetres while constructing. Oval bow with an off-centre through hole.
bow=scale(Point(0,0).buffer(1,quad_segs=32),10.5,11.5)
blade=Polygon([(6,-4),(11,-4),(11,-3.4),(15,-3.4),(16.5,-1.8),(18,-1.8),(19.5,-3.4),(22,-3.4),(23.7,-.7),(25,-.7),(27,-3.4),(30,-3.4),(31.5,-1.6),(33,-1.6),(34.8,-3.4),(38,-3.4),(39,-2),(43,-2),(45,0),(43,3.4),(11,3.4),(11,4),(6,4)])
shape=bow.union(blade).difference(Point(-3,3).buffer(3.2,quad_segs=24))
# Bevel as thin contour layers, united into one watertight mesh.
layers=[]
for z,h,inset in [(-1,.15,.20),(-.85,.15,.07),(-.70,1.40,0),(.70,.15,.07),(.85,.15,.20)]:
 m=trimesh.creation.extrude_polygon(shape.buffer(-inset,join_style=2),h,engine='earcut');m.apply_translation([0,0,z]);layers.append(m)
mesh=trimesh.boolean.union(layers,engine='manifold')
# Mill-cut shallow long grooves, both faces. Cutters start beyond shoulder.
cutters=[]
for z in [.87,-.87]:
 for y,w in [(.55,1.05),(2.20,.42)]:
  c=trimesh.creation.box(extents=[33,w,.42]);c.apply_translation([29,y,z]);cutters.append(c)
mesh=trimesh.boolean.difference([mesh,*cutters],engine='manifold')
mesh.merge_vertices();mesh.remove_unreferenced_vertices();mesh.fix_normals()
assert mesh.is_watertight and mesh.is_winding_consistent and mesh.volume>0
# Physical dimensions in metres; flat on XZ, thickness along Y for Unity.
v=mesh.vertices.copy(); mesh.vertices=np.column_stack((v[:,0],v[:,2],-v[:,1]))*.001
mesh.apply_translation(-mesh.bounds.mean(axis=0))
# Procedural tarnished nickel/brass texture; this is a new material, not the source photo.
rng=np.random.default_rng(97); n=1024
noise=rng.normal(0,4,(n,n,1)); coarse=Image.fromarray(rng.integers(0,255,(64,64),dtype=np.uint8)).resize((n,n),Image.Resampling.BICUBIC).filter(ImageFilter.GaussianBlur(9))
grain=(np.array(coarse).astype(float)-128)/14
arr=np.clip(np.array([164,151,123])[None,None,:]+noise+grain[:,:,None],0,255).astype(np.uint8)
im=Image.fromarray(arr);draw=ImageDraw.Draw(im)
for i in range(1600):
 x,y=rng.integers(0,n,2);l=int(rng.integers(1,25));draw.line((x,y,x+l,y+int(rng.integers(-3,4))),fill=(184,174,149),width=1)
im.save(out/'Key_BaseColor.png')
uv=np.column_stack(((mesh.vertices[:,0]-mesh.bounds[0,0])/mesh.extents[0],(mesh.vertices[:,2]-mesh.bounds[0,2])/mesh.extents[2]))
mat=trimesh.visual.material.PBRMaterial(name='Aged_Nickel',baseColorTexture=im,metallicFactor=.8,roughnessFactor=.48)
mesh.visual=trimesh.visual.texture.TextureVisuals(uv=uv,material=mat)
mesh.export(out/'Chia_Khoa.glb')
# Portable OBJ with explicit UVs and face normals.
with open(out/'Chia_Khoa.obj','w') as f:
 f.write('# Dimensions in metres. Y up.\nmtllib Chia_Khoa.mtl\no Chia_Khoa\n')
 for p in mesh.vertices:f.write('v %.8f %.8f %.8f\n'%tuple(p))
 for p in uv:f.write('vt %.8f %.8f\n'%tuple(p))
 for p in mesh.face_normals:f.write('vn %.8f %.8f %.8f\n'%tuple(p))
 f.write('usemtl Aged_Nickel\ns off\n')
 for i,t in enumerate(mesh.faces):f.write('f '+' '.join(f'{j+1}/{j+1}/{i+1}' for j in t)+'\n')
(out/'Chia_Khoa.mtl').write_text('newmtl Aged_Nickel\nKa 0.12 0.11 0.09\nKd 1 1 1\nKs 0.65 0.65 0.65\nNs 90\nd 1\nillum 2\nmap_Kd Key_BaseColor.png\n')
# Actual mesh preview, including reverse and narrow edge.
fig=plt.figure(figsize=(12,7),facecolor='#202329')
for k,(elev,azim,title) in enumerate([(62,-65,'MẶT TRÊN'),(-48,-60,'MẶT DƯỚI'),(12,-65,'ĐỘ DÀY VÀ RÃNH')]):
 ax=fig.add_subplot(2,2,k+1,projection='3d',facecolor='#202329')
 pts=mesh.vertices[:,[0,2,1]]*1000
 norms=mesh.face_normals[:,[0,2,1]];light=np.array([-.3,-.4,.85]);light/=np.linalg.norm(light)
 shade=.4+.6*np.abs(norms@light)
 colors=np.clip(np.array([.70,.64,.49])[None,:]*shade[:,None],0,1)
 ax.add_collection3d(Poly3DCollection(pts[mesh.faces],facecolors=colors,edgecolor='none'))
 ax.set_xlim(-29,29);ax.set_ylim(-13,13);ax.set_zlim(-3,3);ax.set_box_aspect((58,26,6));ax.view_init(elev=elev,azim=azim);ax.set_axis_off();ax.set_title(title,color='white',fontsize=11)
fig.text(.57,.28,'CHÌA KHÓA CỬA\nBản dựng thử 3D\n\nLỗ xuyên • Răng chìa • Rãnh hai mặt\nDài 55,5 mm • Dày 2 mm',color='white',fontsize=14,linespacing=1.7)
fig.savefig(out/'Xem_Truoc.png',dpi=150,bbox_inches='tight',facecolor=fig.get_facecolor());plt.close(fig)
(out/'HUONG_DAN.txt').write_text('''CHÌA KHÓA CỬA – BẢN DỰNG THỬ 3D

Đây là mô hình dựng lại theo dáng chìa khóa, không phải quét chính xác ảnh.
Có lỗ xuyên, răng chìa và rãnh hai mặt; vật liệu kim loại xỉn dựng mới.
Kích thước: dài khoảng 5,55 cm; dày 2 mm. Đơn vị mô hình là mét.

ĐƯA VÀO UNITY
1. Giải nén gói tải về.
2. Tạo thư mục Assets/ChiaKhoa, kéo Chia_Khoa.obj, Chia_Khoa.mtl và Key_BaseColor.png vào cùng thư mục đó.
3. Kéo Chia_Khoa.obj từ cửa sổ Project vào Scene. Mô hình nhỏ đúng kích thước thật; chọn vật thể và nhấn F để nhìn gần.
4. Nếu vật liệu không tự nhận hoặc bị hồng: tạo Material mới, chọn shader phù hợp dự án (URP/Lit với URP, Standard với Built-in). Kéo Key_BaseColor.png vào Base Map/Albedo; Metallic khoảng 0.8, Smoothness khoảng 0.5; gán vật liệu lên mô hình.
5. Muốn thử nhìn lớn: tăng Scale cả ba trục lên 5 hoặc 10; khi dùng thực tế trả về 1.
6. Muốn có va chạm: thêm Box Collider. Muốn rơi: thêm Rigidbody và kiểm tra collider.

Chia_Khoa.glb là bản xem ở phần mềm hỗ trợ glTF, có vật liệu nhúng; Unity có thể cần gói nhập glTF. Dùng OBJ để nhập đơn giản.
Xem_Truoc.png là ảnh dựng từ chính mô hình.
build_key.py là mã nguồn dựng lại (Python; cần numpy, trimesh, shapely, mapbox-earcut, manifold3d, pillow, matplotlib).
Chưa chạy thử trong Unity. Chưa có chức năng nhặt vật phẩm hoặc mở cửa.
''',encoding='utf-8')
check=trimesh.load(out/'Chia_Khoa.obj',force='mesh',process=True)
check.merge_vertices(merge_tex=True,merge_norm=True)
assert check.is_watertight and check.is_winding_consistent
summary={'vertices':len(mesh.vertices),'triangles':len(mesh.faces),'watertight':bool(mesh.is_watertight),'dimensions_m':mesh.extents.tolist()}
(out/'Thong_So.json').write_text(json.dumps(summary,indent=2))
with zipfile.ZipFile(out.parent/'Chia_Khoa_3D_Unity.zip','w',zipfile.ZIP_DEFLATED) as z:
 for p in sorted(out.iterdir()):z.write(p,'Chia_Khoa_3D/'+p.name)
print(summary)
