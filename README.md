# Parallel and Distributed Computing UFSCar's course final project

### English
This project is based on moving spheres around the scene, calculating positions with three different methods. The first utilizes a sequential approach, only with the main thread making all the calculations for each sphere. For the second, the calculations were distributed across all CPU cores and threads, using Unity’s Job System. And for the last method, we explored the high amount of GPU cores to show its high computing power.  
As calculating position is a fairly simple task, the performance boost is not initially seen. So, to find the limits of calculation for each approach, an iterator was used to replicate complex calculations, as shown in the bottom left corner, making this task N times, controlled by the slider on screen.

With only a couple of hundreds of iterations, the sequential code already struggles, once that for each sphere there’s an iteration of calculations, becoming a lot of calculations really fast. When it comes to the Jobs system (noting that I’ve got 10 cores and 16 threads), the workload is distributed between the 15 threads (as the main thread controls them), making the number of calculations per thread lower, resulting in a higher number of possible iterations. At last, using Unity’s Compute Shader system, it was possible to make all of these sphere calculations using the GPU, being naturally parallel devices, seeing the number of cores and memory access method. 
In terms of comparison, my CPU has only 10 cores, while my GPU has 2048 of them. With that in mind, the workload is divided into really small tasks for each GPU core, that can calculate everything really fast and drastically reduce computational time. Besides that, we had a different approach to the sphere movement on this last method, to better see how the threads, blocks and grids work on the GPU, making a visual difference between each of them, making the learning of those topics easier. 
In this case, we’ve got a 4x4 grid, totalizing 16 blocks, and each of these with a 16x16 configuration, having 256 threads per block. The visual representation shows each block separated from each other for better understanding, in which each block also moves at a different speed, being able to see how the GPU is working.


If anyone wants to learn more about this topic, this CUDA post from NVIDIA shows in detail these thread/block/grid arrangements and how to use GPU to make these calculations (even though Unity doesn’t use CUDA, the logic is the same). Link: https://developer.nvidia.com/blog/even-easier-introduction-cuda/



It’s worth noting that the repository is still a work in progress, so any help would be appreciated!


### Português (Brasil)
Esse projeto envolveu a movimentação de esferas pela cena, fazendo cálculos da posição de três métodos diferentes. Tem-se a primeira de modo sequencial, apenas com a main thread realizando todos os cálculos para cada uma das esferas. Então, dividiram-se os cálculos entre as threads do processador, por meio do Jobs e, por último, todos os cálculos foram realizados pelas threads na GPU. 
Como o cálculo de posição é simples, a performance não apresenta melhoras significativas inicialmente. Então, para explorar os limites de cálculos em cada método, utilizamos um iterador, de modo a permitir a realização de cálculos mais complexos, os quais podem ser observados no canto inferior direito da tela, N vezes, sendo alterado pelo slider mostrado na tela. 

É possível ver que com poucas centenas de iterações, o código sequencial já se torna bem lento, uma vez que para cada esfera há a iteração de cálculos, tornando-se custoso rapidamente. Ao trocar para o sistema de Jobs (tendo em vista que tenho 10 cores e 16 threads na minha máquina), o trabalho se divide entre as 15 threads, além da principal, reduzindo a quantidade de esferas por thread e o elevando número de iterações possíveis. Por fim, utilizando o sistema de Compute shader da Unity, foi possível a realização de todos os cálculos das esferas na GPU. 
GPUs são dispositivos naturalmente paralelos, desde o número de núcleos até o método de acesso à memória. Para termos de comparação, minha CPU tem 10 cores, enquanto minha GPU tem 2048 cores. Desse modo, o trabalho é divido em tasks muito menores, que podem ser feitas rapidamente pelos cores da GPU, diminuindo drasticamente o tempo total dos cálculos e elevando a performance. Além disso, foi empregado um sistema de movimentação diferente para este último caso, a fim de se obter uma melhor visualização de como threads, blocos e grids se comportam na GPU, de modo a ilustrar esses comportamentos e facilitar a compreensão. Neste caso, temos grids de 4x4 com um total de 16 blocos, estes com disposição 16x16, com um total de 256 threads por bloco, com tal divisão de blocos representada visualmente - nesse cenário, cada um se move em velocidade diferente entre o grid.
Também é possível alterar a disposição do grid e dos blocos facilmente pelo inspetor, assim como fazer a movimentação do compute shader ser igual aos outros métodos, apenas descomentando algumas linhas. 


Se quiserem conhecer mais sobre, esta referência sobre CUDA explica com mais detalhes a divisão e a utilização da GPU para cálculos (mesmo que a Unity não utilize CUDA, o entendimento e funcionamento é muito semelhante). Link: https://developer.nvidia.com/blog/even-easier-introduction-cuda/



Vale ressaltar que o repositório ainda está em processo de organização, então toda ajuda é bem vinda para deixá-lo ainda melhor!
