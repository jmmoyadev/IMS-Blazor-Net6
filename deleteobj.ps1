$source="./"
$strings=@("obj", "bin", "packages", "node_modules", ".vs", ".git", "temp", "tmp")

cd ($source); get-childitem -Include ($strings) -Recurse -force | Remove-Item -Force -Recurse

