//Written for Drakensang Online. https://store.steampowered.com/app/2067850/
using System.IO.Compression;

FileStream input = File.OpenRead(args[0]);
BinaryReader br = new(input);
if (new string(br.ReadChars(8)) != "_B3NHB3N")
    throw new Exception("Not a Drakensang Online bundle file.");

int fileCount = br.ReadInt32();
int nameTableStart = br.ReadInt32();
int unknownTableStart = br.ReadInt32();
int fileTableStart = br.ReadInt32();

MemoryStream nameTable = new MemoryStream(br.ReadBytes(unknownTableStart - nameTableStart));
MemoryStream unknownTable = new MemoryStream(br.ReadBytes(fileTableStart - unknownTableStart));
MemoryStream fileTable = new MemoryStream(br.ReadBytes((int)br.BaseStream.Length - fileTableStart));
br.Close();

BinaryReader ntr = new(nameTable);
BinaryReader utr = new(unknownTable);
BinaryReader ftr = new(fileTable);

/*br.BaseStream.Position = nameTableStart;
utr.BaseStream.Position = unknownTableStart;
ftr.BaseStream.Position = fileTableStart;*/

for (int i = 0; i < fileCount; i++)
{
    string name = new(ntr.ReadChars(ntr.ReadInt16()));
    name = name.Replace("/", "//");
    int unknown1 = utr.ReadInt32();
    utr.ReadBytes(32);
    int size = utr.ReadInt32();
    int start = utr.ReadInt32();
    ftr.BaseStream.Position = start;
    int unknown4 = ftr.ReadInt32();
    int unknown5 = ftr.ReadInt32();
    Directory.CreateDirectory(Path.GetDirectoryName(args[0]) + "//" + Path.GetFileNameWithoutExtension(args[0]) + "//" + Path.GetDirectoryName(name));
    ftr.ReadInt16();
    using var ds = new DeflateStream(new MemoryStream(ftr.ReadBytes(size - 2)), CompressionMode.Decompress);
    ds.CopyTo(File.Create(Path.GetDirectoryName(args[0]) + "//" + Path.GetFileNameWithoutExtension(args[0]) + "//" + name));
}
