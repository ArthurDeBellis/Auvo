# Auvo

## Lógicas empregadas:

### Leitura de CSV
A leitura do CSV foi feita a partir da biblioteca CsvHelper.

### Cálculo das horas trabalhadas e horas extras
A lógica deste cálculo foi feita usando DateTime, onde é verificado se o dia de trabalho do funcionário, apresentado no CSV é sábado ou domingo, caso seja, é contado como dia extra. O cálculo da quantidade de dias úteis no mês é feita de forma análoga, de acordo com o ano e o mês presentes no nome do arquivo.
Neste cálculo, caso um dia de trabalho do funcionário seja no sábado ou domingo, todas as horas do dia são contadas como horas extras. Outra forma de obter as horas extras é a partir dos dias que as horas trabalhadas passam de 8, neste caso, todas as horas além de 8 são consideradas extras.

### Geração do arquivo Json
Após obtenção das informações de cada departamento, o arquivo JSON é gerado usando a biblioteca Json.

### Paralelismo
O paralelismo é feito a partir de tasks, que são usadas para os cálculos das informações de cada funcionário presente em cada um dos arquivos CSV lidos.
