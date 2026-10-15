# Ataque de projétil

O prefab `Player & Camera` já contém os componentes no filho `Player`.
Clique esquerdo dispara uma bola pela mira central; segurar não repete.
O mouse orienta a câmera e o visual do personagem, inclusive parado.

## Configuração

- `IceBallSettings.asset`: velocidade (20), dano (10), duração (5 s), intervalo (0,35 s), raio de colisão (0,15) e máscara de impacto.
- `Prefabs/Combat/IceBall.prefab`: esfera provisória. O movimento e a colisão são calculados por varreduras esféricas no `FixedUpdate`; não precisa de Rigidbody nem Collider.
- `CastPoint`, filho da raiz visual: posição de saída. O lançador verifica também o caminho do corpo até esse ponto, evitando disparos através de paredes.
- `CameraAimProvider`: câmera real, distância máxima da mira (100) e `Aim Viewport Position`. X e Y variam entre 0 e 1; (0,5; 0,5) é o centro, aumentar X move para a direita e aumentar Y move para cima. O desenho e o disparo compartilham essa posição.
- `PlayerInput`: ação `Attack` no mapa `Player`, recebida por `PlayerAttackInput` em Send Messages.

O lançador guarda sua própria recarga. O ScriptableObject contém somente dados compartilhados. O projétil copia esses dados ao nascer e não depende da câmera ou do input.

Para receber dano, um componente no alvo ou em seu pai deve implementar `IDamageable.TakeDamage(float amount, GameObject source)`. O alvo precisa de collider não-trigger incluído na máscara. Paredes interrompem o projétil mesmo sem implementar dano. Colliders do próprio jogador são ignorados. Não há congelamento nesta versão.

`ProjectileCaster.onCast` e `MagicProjectile.onImpact` permitem conectar efeitos posteriormente. O impacto é emitido antes da destruição do projétil; efeitos persistentes devem ser instanciados separadamente. `AimReticle` é apenas uma cruz provisória e pode ser substituído por UI.

## Verificação no Play Mode

1. Abrir uma cena com o prefab e conferir a ausência de erros no Console.
2. Mirar parado, andar lateralmente, recuar e pular; conferir que câmera e movimento continuam funcionando.
3. Clicar uma vez: sai uma esfera. Segurar: não repete. Cliques dentro de 0,35 s são descartados.
4. Mover a mira após o disparo: a bola mantém sua trajetória, incluindo tiros para cima e para baixo.
5. Atirar contra parede fina e encostado em uma parede: não deve atravessar nem nascer do outro lado.
6. Atirar para o céu: a esfera desaparece após 5 s. Verificar que nunca colide com o próprio jogador.
7. Usar um alvo com IDamageable: cada impacto deve aplicar dano uma única vez.

A compilação externa não pôde ser concluída nesta sessão por restrição de acesso ao SDK. Play Mode ainda precisa ser validado no Editor.
