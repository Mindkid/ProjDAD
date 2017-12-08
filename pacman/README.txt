---------------------------------------------------
---------------------------------------------------
----------------  PROJECTO DAD --------------------
---------------------------------------------------
---------------------------------------------------
------------------ GRUPO 3 ------------------------
------------ Rui Oliveira 70604  ------------------
------------ Paulo Martins 70608 ------------------
---------------------------------------------------

O PuppetMaster funciona em modo batch e em modo 
interactivo.

COMO TESTAR:
	1 - Ir a PCS\bin\Release\
	2 - Executar PCS.exe
	3 - Voltar ao directório raiz (pacman)
	4 - Abrir terminal na pasta
	5 - Fazer o seguinte comando:
		cd PuppetMaster\bin\Release
	6 - Depois executar o commando:
		PuppetMaster.exe test.txt	
	7 - Esperar que o servidor e os jogos comecem 
	8 - Desfrutar do jogo

COISAS EM FALTA:
	-> Comunicação Assincrona entre o PCS e o
		PuppetMaster.
	-> Implementar a interface IProcessToPCS
		tanto o cliente como no servidor
	-> Tolerância de faltas.
	-> Adicionar alguns detalhes de implementação
		que embora estejam lá não são actualizados
		de forma dinamica.
--------------------------------------------------	
--------------------------------------------------	